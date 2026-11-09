using UnityEngine;
using UnityEngine.InputSystem;

namespace BapakSimulator
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonWalker : MonoBehaviour
    {
        [SerializeField] private Camera view;
        [SerializeField] private float speed = 2.5f;
        [SerializeField] private float mouseSensitivity = 0.12f;
        [SerializeField] private float touchSensitivity = 0.15f;
        private CharacterController body;
        private Vector3 spawn;
        private float pitch;
        private float fallSpeed;

        public void Configure(Camera camera) => view = camera;

        private void Awake()
        {
            body = GetComponent<CharacterController>();
            spawn = transform.position;
        }

        private void OnEnable()
        {
            if (!Application.isMobilePlatform) Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable() => Cursor.lockState = CursorLockMode.None;

        private void Update()
        {
            Vector2 move = Vector2.zero;
            Vector2 look = Vector2.zero;
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
                Cursor.lockState = CursorLockMode.None;
            if (!Application.isMobilePlatform && Cursor.lockState != CursorLockMode.Locked)
            {
                if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                    Cursor.lockState = CursorLockMode.Locked;
                return;
            }
            if (keyboard != null)
                move = new Vector2((keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0),
                    (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0));
            if (mouse != null && Cursor.lockState == CursorLockMode.Locked)
                look = mouse.delta.ReadValue() * mouseSensitivity;
            if (Touchscreen.current != null)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (!touch.press.isPressed) continue;
                    if (touch.startPosition.ReadValue().x < Screen.width * 0.5f)
                        move = Vector2.ClampMagnitude((touch.position.ReadValue() - touch.startPosition.ReadValue()) / (Screen.height * 0.12f), 1);
                    else
                        look += touch.delta.ReadValue() * touchSensitivity;
                }
            }
            Step(move, look, Time.deltaTime);
            if (transform.position.y < -5) ResetPosition();
        }

        public void Step(Vector2 move, Vector2 look, float deltaTime)
        {
            transform.Rotate(0, look.x, 0);
            pitch = Mathf.Clamp(pitch - look.y, -80, 80);
            view.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
            fallSpeed = body.isGrounded ? -2 : fallSpeed + Physics.gravity.y * deltaTime;
            Vector3 direction = transform.TransformDirection(new Vector3(move.x, 0, move.y));
            body.Move((Vector3.ClampMagnitude(direction, 1) * speed + Vector3.up * fallSpeed) * deltaTime);
        }

        private void ResetPosition()
        {
            body.enabled = false;
            transform.position = spawn;
            body.enabled = true;
            fallSpeed = 0;
        }

        private void OnGUI()
        {
            // ponytail: Indonesian-only prototype HUD; add language selection with the first dialogue/UI milestone.
            var rect = Screen.safeArea;
            GUI.Box(new Rect(rect.x + 12, Screen.height - rect.yMax + 12, Mathf.Min(600, rect.width - 24), 64),
                Application.isMobilePlatform
                    ? "UJI JALAN KONTRAKAN\nGeser kiri: jalan | Geser kanan: lihat"
                    : "UJI JALAN KONTRAKAN\nWASD: jalan | Mouse: lihat | Esc: lepas kursor | Klik: lanjut");
        }
    }
}
