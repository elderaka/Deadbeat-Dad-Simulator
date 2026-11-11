using UnityEngine;
using UnityEngine.InputSystem;

namespace BapakSimulator
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera view;
        [SerializeField] private TouchControls touchControls;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputAction switchVehicleView;
        [SerializeField, Min(0)] private float jumpHeight = 0.75f;
        [SerializeField, Min(0)] private float speed = 2.5f;
        [SerializeField] private float mouseSensitivity = 0.12f;
        [SerializeField] private float touchSensitivity = 0.15f;
        [SerializeField, Min(0)] private float interactionDistance = 2f;
        [SerializeField] private LayerMask interactionLayers = Physics.DefaultRaycastLayers;

        private CharacterController body;
        private RaycastHit[] interactionHits = new RaycastHit[8];
        private InputAction ownedJumpAction;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private Quaternion neutralViewRotation;
        private float pitch;
        private float cameraYaw;
        private float verticalSpeed;
        private PauseMenu pauseMenu;
        private bool gameplayInputBlocked;
        private bool sequenceInputBlocked;
        private bool dialogueLookActive;
        private bool preferChaseView;
        private bool chaseView;
        private bool worldInteractionBlocked;
        private int resumeInteractionFrame;
        private int? movementTouchId;
        private int? lookTouchId;

        public bool IsPaused { get; private set; }
        internal bool UsesTouchControls => touchControls != null && touchControls.isActiveAndEnabled && touchControls.ModeEnabled;
        internal bool HasPauseMenu => pauseMenu != null && pauseMenu.isActiveAndEnabled;

        internal bool CanOpenDialogue => !sequenceInputBlocked && CanOpenSequenceDialogue;

        internal bool CanOpenSequenceDialogue => isActiveAndEnabled && ActiveVehicle == null &&
            !IsPaused && !gameplayInputBlocked && !worldInteractionBlocked;

        internal void SetSequenceInputBlocked(bool blocked)
        {
            sequenceInputBlocked = blocked;
            if (!blocked) resumeInteractionFrame = Time.frameCount + 1;
            ClearTouches();
        }

        public VehicleDrive ActiveVehicle { get; private set; }
        private Transform walkingParent;
        private Transform walkingCameraParent;
        private Vector3 walkingCameraPosition;

        public void Configure(Camera camera, InputActionReference jump = null)
        {
            view = camera;
            if (jump != null) jumpAction = jump;
            Initialize();
        }

        private void Awake()
        {
            if (view == null)
            {
                Debug.LogError("PlayerController requires a first-person camera.", this);
                enabled = false;
                return;
            }

            Initialize();
        }

        private void Initialize()
        {
            body = GetComponent<CharacterController>();
            pauseMenu = GetComponent<PauseMenu>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
            neutralViewRotation = view.transform.localRotation;
            pitch = 0;
            cameraYaw = 0;
            verticalSpeed = 0;
        }

        private void OnEnable()
        {
            switchVehicleView?.Enable();
            var action = jumpAction?.action;
            if (action != null && !action.enabled)
            {
                action.Enable();
                ownedJumpAction = action;
            }

            if (!Application.isMobilePlatform && Application.isFocused)
                Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            switchVehicleView?.Disable();
            ownedJumpAction?.Disable();
            ownedJumpAction = null;

            if (!Application.isMobilePlatform)
                Cursor.lockState = CursorLockMode.None;

            ClearTouches();
        }

        private void OnApplicationFocus(bool focused)
        {
            if (focused) return;

            ClearTouches();
            if (!Application.isMobilePlatform)
                Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            if (IsPaused || sequenceInputBlocked || Time.frameCount < resumeInteractionFrame) return;
            if (gameplayInputBlocked)
            {
                if (ActiveVehicle != null && ActiveVehicle.CanUseChaseView &&
                    switchVehicleView?.WasPressedThisFrame() == true)
                {
                    preferChaseView = !chaseView;
                    SetDrivingView(preferChaseView);
                }
                ReadCameraOnlyInput();
                return;
            }

            Vector2 move = Vector2.zero;
            Vector2 look = Vector2.zero;
            bool interact = false;
            bool jump = false;

            if (Application.isFocused && Time.deltaTime > 0)
            {
                if (UsesTouchControls && !worldInteractionBlocked)
                {
                    touchControls.Read(out move, out look, out jump);
                    look *= touchSensitivity;
                }
                else if (Application.isMobilePlatform)
                    ReadTouch(out move, out look, out interact);
                else
                    ReadDesktop(out move, out look, out interact);
            }

            jump |= Application.isFocused && Cursor.lockState == CursorLockMode.Locked &&
                jumpAction?.action?.WasPressedThisFrame() == true;
            Step(move, look, Time.deltaTime, jump);
            if (interact) TryInteract();
        }

        public bool BeginDriving(VehicleDrive vehicle, Transform seat)
        {
            if (ActiveVehicle != null || IsPaused || sequenceInputBlocked || gameplayInputBlocked || !isActiveAndEnabled) return false;

            ActiveVehicle = vehicle;
            SetGameplayInputBlocked(true);
            walkingParent = transform.parent;
            walkingCameraParent = view.transform.parent;
            walkingCameraPosition = view.transform.localPosition;
            body.enabled = false;
            transform.SetParent(seat, false);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            SetDrivingView(preferChaseView);
            return true;
        }

        private void SetDrivingView(bool useChaseView)
        {
            chaseView = useChaseView && ActiveVehicle.CanUseChaseView;
            pitch = chaseView ? ActiveVehicle.ChasePitch : 0f;
            cameraYaw = 0f;
            view.transform.SetParent(chaseView ? ActiveVehicle.ChasePivot : ActiveVehicle.DriverCameraMount, false);
            view.transform.localPosition = Vector3.zero;
            ApplyCameraLook(Vector2.zero);
        }

        public bool CanExitVehicle(Vector3 position)
        {
            float radius = body.radius * 0.9f;
            Vector3 feet = position + body.center - Vector3.up * (body.height * 0.5f);
            return !Physics.CheckCapsule(feet + Vector3.up * (radius + 0.03f),
                feet + Vector3.up * (body.height - radius), radius,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        }

        public void EndDriving(Vector3 position, Quaternion rotation)
        {
            transform.SetParent(walkingParent, true);
            transform.SetPositionAndRotation(position, Quaternion.Euler(0, rotation.eulerAngles.y, 0));
            view.transform.SetParent(walkingCameraParent, false);
            view.transform.SetLocalPositionAndRotation(walkingCameraPosition, neutralViewRotation);
            pitch = 0;
            cameraYaw = 0;
            verticalSpeed = 0;
            body.enabled = true;
            ActiveVehicle = null;
            chaseView = false;
            SetGameplayInputBlocked(false);
            if (UsesTouchControls) Cursor.lockState = CursorLockMode.None;
        }

        internal void SetPaused(bool paused)
        {
            IsPaused = paused;
            ClearTouches();
            if (!paused) resumeInteractionFrame = Time.frameCount + 1;
        }

        public void SetGameplayInputBlocked(bool blocked)
        {
            gameplayInputBlocked = blocked;
            if (blocked) ClearTouches();
        }

        internal void SetDialogueInputBlocked(bool blocked)
        {
            if (!blocked && dialogueLookActive)
            {
                transform.Rotate(0, cameraYaw, 0);
                cameraYaw = 0;
                ApplyCameraLook(Vector2.zero);
            }
            dialogueLookActive = blocked;
            SetGameplayInputBlocked(blocked);
        }

        private void ReadCameraOnlyInput()
        {
            var mouse = Mouse.current;
            if (!Application.isFocused || mouse == null) return;

            if (ActiveVehicle != null)
            {
                if (Cursor.lockState != CursorLockMode.Locked)
                {
                    if (mouse.leftButton.wasPressedThisFrame) Cursor.lockState = CursorLockMode.Locked;
                    return;
                }
            }
            else if (!dialogueLookActive || !mouse.rightButton.isPressed)
                return;

            ApplyCameraLook(mouse.delta.ReadValue() * mouseSensitivity);
        }

        private void ApplyCameraLook(Vector2 look)
        {
            cameraYaw = Mathf.Repeat(cameraYaw + look.x + 180f, 360f) - 180f;
            if (chaseView)
            {
                pitch = Mathf.Clamp(pitch - look.y, -15f, 75f);
                var orbit = Quaternion.Euler(pitch, cameraYaw, 0);
                view.transform.SetLocalPositionAndRotation(orbit * (Vector3.back * ActiveVehicle.ChaseDistance), orbit);
                return;
            }

            pitch = Mathf.Clamp(pitch - look.y, -80f, 80f);
            var neutral = ActiveVehicle != null ? Quaternion.identity : neutralViewRotation;
            view.transform.localRotation = Quaternion.Euler(0, cameraYaw, 0) * neutral * Quaternion.Euler(pitch, 0, 0);
        }

        public void SetWorldInteractionBlocked(bool blocked)
        {
            worldInteractionBlocked = blocked;
            if (blocked) return;

            resumeInteractionFrame = Time.frameCount + 1;
            if (UsesTouchControls && ActiveVehicle == null && !IsPaused)
                Cursor.lockState = CursorLockMode.None;
        }

        private void ReadDesktop(out Vector2 move, out Vector2 look, out bool interact)
        {
            move = Vector2.zero;
            look = Vector2.zero;
            interact = false;
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            if (!HasPauseMenu && keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                return;
            }

            if (Cursor.lockState != CursorLockMode.Locked)
            {
                if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                    Cursor.lockState = CursorLockMode.Locked;

                return;
            }

            if (keyboard != null)
            {
                move = new Vector2(
                    (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0),
                    (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0));
                interact = keyboard.eKey.wasPressedThisFrame;
            }

            if (mouse != null)
                look = mouse.delta.ReadValue() * mouseSensitivity;
        }

        private void ReadTouch(out Vector2 move, out Vector2 look, out bool interact)
        {
            move = Vector2.zero;
            look = Vector2.zero;
            interact = false;
            var touchscreen = Touchscreen.current;
            if (touchscreen == null)
            {
                ClearTouches();
                return;
            }

            bool movementTouchActive = false;
            bool lookTouchActive = false;
            float dragRadius = Mathf.Max(1, Screen.height * 0.12f);
            foreach (var touch in touchscreen.touches)
            {
                int id = touch.touchId.ReadValue();
                Vector2 start = touch.startPosition.ReadValue();
                if (!touch.press.isPressed)
                {
                    if (id == lookTouchId)
                        interact |= touch.tap.wasPressedThisFrame;
                    continue;
                }

                if (start.x < Screen.width * 0.5f)
                {
                    movementTouchId ??= id;
                    if (id != movementTouchId) continue;

                    movementTouchActive = true;
                    move = Vector2.ClampMagnitude((touch.position.ReadValue() - start) / dragRadius, 1);
                }
                else
                {
                    lookTouchId ??= id;
                    if (id != lookTouchId) continue;

                    lookTouchActive = true;
                    look = touch.delta.ReadValue() * touchSensitivity;
                }
            }

            if (!movementTouchActive) movementTouchId = null;
            if (!lookTouchActive) lookTouchId = null;
        }

        private void ClearTouches()
        {
            movementTouchId = null;
            lookTouchId = null;
        }

        public void Step(Vector2 move, Vector2 look, float deltaTime, bool jump = false)
        {
            if (IsPaused || sequenceInputBlocked || gameplayInputBlocked || deltaTime <= 0 || !body.enabled) return;
            if (transform.position.y < -5f)
            {
                ResetPosition();
                return;
            }

            transform.Rotate(0, look.x, 0);
            ApplyCameraLook(new Vector2(0, look.y));

            if (body.isGrounded && verticalSpeed <= 0)
                verticalSpeed = jump && jumpHeight > 0 ? Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight) : -2f;
            else
                verticalSpeed += Physics.gravity.y * deltaTime;
            Vector2 input = Vector2.ClampMagnitude(move, 1);
            Vector3 planarVelocity = (transform.right * input.x + transform.forward * input.y) * speed;
            var collisions = body.Move((planarVelocity + Vector3.up * verticalSpeed) * deltaTime);
            if ((collisions & CollisionFlags.Above) != 0 && verticalSpeed > 0)
                verticalSpeed = 0;
        }

        public WorldInteractable FocusedInteractable
        {
            get
            {
                if (ActiveVehicle != null || IsPaused || sequenceInputBlocked || gameplayInputBlocked || worldInteractionBlocked || Time.frameCount < resumeInteractionFrame) return null;
                var cameraTransform = view.transform;
                int count;
                do
                {
                    count = Physics.RaycastNonAlloc(cameraTransform.position, cameraTransform.forward, interactionHits,
                        interactionDistance, interactionLayers, QueryTriggerInteraction.Ignore);
                    if (count == interactionHits.Length) System.Array.Resize(ref interactionHits, count * 2);
                    else break;
                } while (true);

                Collider nearest = null;
                float distance = float.PositiveInfinity;
                for (int i = 0; i < count; i++)
                {
                    var hit = interactionHits[i];
                    if (hit.distance >= distance || hit.collider.transform.IsChildOf(transform)) continue;
                    nearest = hit.collider;
                    distance = hit.distance;
                }

                var target = nearest != null ? nearest.GetComponentInParent<WorldInteractable>() : null;
                return target != null && target.isActiveAndEnabled ? target : null;
            }
        }

        public bool TryInteract() => FocusedInteractable?.TryInteract() ?? false;

        private void ResetPosition()
        {
            body.enabled = false;
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            view.transform.localRotation = neutralViewRotation;
            body.enabled = true;
            pitch = 0;
            cameraYaw = 0;
            verticalSpeed = 0;
            ClearTouches();
        }

        [SerializeField] private bool showTestOverlay = true;

        public string VehicleCameraHint =>
            ActiveVehicle != null && ActiveVehicle.CanUseChaseView && switchVehicleView != null
                ? $"{switchVehicleView.GetBindingDisplayString()}  •  {(chaseView ? "Kamera belakang" : "Kamera pengemudi")}"
                : string.Empty;

        private void OnGUI()
        {
            if (!showTestOverlay) return;
            var rect = Screen.safeArea;
            if (ActiveVehicle != null && ActiveVehicle.CanUseChaseView && switchVehicleView != null)
            {
                string binding = switchVehicleView.GetBindingDisplayString();
                GUI.Box(new Rect(rect.x + 12, Screen.height - rect.yMax + 12, 360, 36),
                    $"{binding}: ganti kamera • {(chaseView ? "Kamera belakang" : "Sudut pandang pengemudi")}");
                return;
            }

            GUI.Box(new Rect(rect.x + 12, Screen.height - rect.yMax + 12, Mathf.Min(640, rect.width - 24), 64),
                Application.isMobilePlatform
                    ? "UJI JALAN KONTRAKAN\nGeser kiri: jalan | Geser kanan: lihat | Ketuk kanan: interaksi"
                    : "UJI JALAN KONTRAKAN\nWASD: jalan | Spasi: lompat | Mouse: lihat | E: interaksi | Esc: " + (HasPauseMenu ? "jeda" : "lepas kursor | Klik: lanjut"));
        }
    }
}
