using UnityEngine;
using UnityEngine.InputSystem;

namespace BapakSimulator
{
    public sealed class FishingRod : WorldInteractable
    {
        public enum Phase { Idle, Ready, Casting, Waiting, Biting, Retrieving, Hooked }

        [SerializeField] private PickupDay household;
        [SerializeField] private ItemInventory catchBag;
        [SerializeField] private PlayerController player;
        [SerializeField] private Camera aimCamera;
        [SerializeField] private FishingTuning tuning;
        [SerializeField] private PondWater water;
        [SerializeField] private Collider landingBank;
        [SerializeField] private Transform rodTip;
        [SerializeField] private GameObject rodVisual;
        [SerializeField] private Transform floatVisual;
        [SerializeField] private LineRenderer line;
        [SerializeField, Min(0.1f)] private float attractionRadius;
        private FishingFish fish;
        [SerializeField] private InputActionReference castAndReel;
        [SerializeField] private InputActionReference putAway;
        [SerializeField, Min(0.1f)] private float shoreRadius;

        private Phase phase;
        private float phaseTime;
        private int equippedFrame;
        private bool reelArmed;
        private FishingFloat bobber;
        private float castTimeout;
        private Vector3 fightAnchor;
        private Vector3 liftPoint;
        private bool lifting;
        private bool clearedBank;
        private Vector3 waterPoint;
        private InputAction ownedCast;
        private InputAction ownedExit;
        private string castHint;
        private string exitHint;

        private FishingFight fight;
        private Quaternion restRodRotation;

        public Phase CurrentPhase => phase;
        public float LineTension => fight?.Tension ?? 0;
        public float LineLength => fight?.LineLength ?? 0;
        public bool FishIsRunning => fight?.IsRunning ?? false;
        public string ReelBinding => castHint;
        public string ExitBinding => exitHint;
        public int CatchCount => household.CatchesToday;

        private void Awake()
        {
            if (household == null || catchBag == null || player == null || aimCamera == null || tuning == null ||
                !tuning.IsValid || water == null || rodTip == null || rodVisual == null ||
                floatVisual == null || line == null || attractionRadius <= 0 ||
                castAndReel?.action == null || putAway?.action == null || shoreRadius <= 0)
            {
                Debug.LogError($"{nameof(FishingRod)} on '{name}' has incomplete fishing setup.", this);
                enabled = false;
                return;
            }

            bobber = floatVisual.GetComponent<FishingFloat>();
            if (bobber == null)
            {
                Debug.LogError($"{nameof(FishingRod)} on '{name}' needs a physical float.", this);
                enabled = false;
                return;
            }

            restRodRotation = rodVisual.transform.localRotation;
            SetLineVisible(false);
            rodVisual.SetActive(false);
        }

        public override bool TryInteract()
        {
            if (!isActiveAndEnabled || phase != Phase.Idle || player.ActiveVehicle != null) return false;
            if (Application.isMobilePlatform)
            {
                household.ReportActivity("Kontrol pancing sentuh belum tersedia.");
                return false;
            }

            if (!household.IsDayRunning)
            {
                household.ReportActivity("Mulai hari di Rumah sebelum memancing.");
                return false;
            }

            phase = Phase.Ready;
            equippedFrame = Time.frameCount;
            ownedCast = EnableIfNeeded(castAndReel.action);
            ownedExit = EnableIfNeeded(putAway.action);
            player.SetWorldInteractionBlocked(true);
            rodVisual.SetActive(true);
            castHint = BindingLabel(castAndReel.action);
            exitHint = BindingLabel(putAway.action);
            household.ReportActivity($"Arahkan ke air. {castHint}: lempar • {exitHint}: selesai.");
            return true;
        }

        private static string BindingLabel(InputAction action) => action.GetBindingDisplayString(
            InputBinding.DisplayStringOptions.DontIncludeInteractions, "Keyboard&Mouse");

        private static InputAction EnableIfNeeded(InputAction action)
        {
            if (action.enabled) return null;
            action.Enable();
            return action;
        }

        private void Update()
        {
            if (phase == Phase.Idle || player.IsPaused) return;

            if (!household.IsDayRunning || player.ActiveVehicle != null ||
                (player.transform.position - transform.position).sqrMagnitude > shoreRadius * shoreRadius)
            {
                PutAway();
                return;
            }

            bool canReadInput = Application.isFocused && Cursor.lockState == CursorLockMode.Locked &&
                Time.frameCount > equippedFrame;
            if (canReadInput && putAway.action.WasPressedThisFrame())
            {
                PutAway();
                household.ReportActivity("Pancing disimpan.");
                return;
            }

            if (phase == Phase.Ready)
            {
                if (canReadInput && castAndReel.action.WasPressedThisFrame())
                {
                    var ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                    if (!TryCast(ray)) household.ReportActivity("Arahkan lemparan ke permukaan kolam.");
                }

                return;
            }

            AdvanceLine(Time.deltaTime, canReadInput && castAndReel.action.IsPressed());
        }

        private bool TryCast(Ray ray)
        {
            if (phase != Phase.Ready || !water.Raycast(ray, out var hit, tuning.CastRange) ||
                Physics.Raycast(ray, hit.distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                return false;

            if (!household.TryBeginFishingCast()) return false;

            Vector3 origin = rodTip.position;
            waterPoint = hit.point;
            float gravity = Mathf.Max(0.01f, -Physics.gravity.y);
            float upSpeed = Mathf.Sqrt(2f * gravity * tuning.CastArc);
            float naturalFlight = (upSpeed + Mathf.Sqrt(upSpeed * upSpeed +
                2f * gravity * Mathf.Max(0, origin.y - waterPoint.y))) / gravity;
            float flight = Mathf.Max(tuning.CastFlightSeconds, naturalFlight);
            SetLineVisible(true);
            bobber.Launch(water, origin, (waterPoint - origin) / flight - Physics.gravity * (0.5f * flight));
            castTimeout = flight + 0.8f;
            reelArmed = false;
            SetPhase(Phase.Casting);
            household.ReportActivity($"Tunggu pelampung bergerak. Tahan {castHint} untuk menggulung.");
            UpdateLine();
            return true;
        }

        private void AdvanceLine(float deltaTime, bool held)
        {
            phaseTime += deltaTime;
            if (!held) reelArmed = true;
            bool reel = held && reelArmed;

            switch (phase)
            {
                case Phase.Casting:
                    if (bobber.IsInWater)
                    {
                        waterPoint = bobber.Position;
                        waterPoint.y = water.SurfaceY;
                        bobber.SetTether(waterPoint);
                        SetPhase(Phase.Waiting);
                    }
                    else if (phaseTime >= castTimeout)
                        FinishCast(false, "Pelampung jatuh di luar kolam.");
                    break;

                case Phase.Waiting:
                    if (reel)
                    {
                        SetPhase(Phase.Retrieving);
                        RetrieveEmptyLine(deltaTime);
                        break;
                    }

                    if (phaseTime < tuning.BiteDelay) break;

                    var hook = waterPoint - Vector3.up * tuning.HookDepth;
                    if (fish == null)
                    {
                        fish = water.FindFish(hook, attractionRadius);
                        if (fish == null) break;
                        fish.Approach(hook);
                    }
                    if ((fish.Position - hook).sqrMagnitude <= 0.0256f)
                    {
                        SetPhase(Phase.Biting);
                        household.ReportActivity($"Pelampung tenggelam! Tahan {castHint} untuk mengait.");
                    }
                    break;

                case Phase.Biting:
                    bobber.SetTether(waterPoint - Vector3.up *
                        (tuning.BiteDip * (0.75f + 0.25f * Mathf.Cos(phaseTime * 18f))));
                    if (reel)
                    {
                        fightAnchor = ShorePoint();
                        fight = new FishingFight(fish.FightTuning, Vector3.Distance(fightAnchor, fish.Position));
                        fish.Hook(fightAnchor, fight.LineLength);
                        SetPhase(Phase.Hooked);
                        household.ReportActivity("Ikan terkait! Lepas gulungan saat ikan lari.");
                    }
                    else if (phaseTime >= tuning.HookWindow)
                    {

                        ReleaseFish();
                        bobber.SetTether(waterPoint);
                        SetPhase(Phase.Waiting);
                        household.ReportActivity("Ikan lepas. Tunggu gigitan berikutnya.");
                    }
                    break;

                case Phase.Retrieving:
                    if (reel) RetrieveEmptyLine(deltaTime);
                    else bobber.SetTether(null);
                    break;

                case Phase.Hooked:
                    FightFish(deltaTime, reel);
                    break;
            }

            if (phase != Phase.Ready) UpdateLine();
        }

        private void RetrieveEmptyLine(float deltaTime)
        {
            bobber.SetTether(Vector3.MoveTowards(bobber.Position, rodTip.position,
                tuning.ReelSpeed * deltaTime + 0.5f));
            if ((bobber.Position - rodTip.position).sqrMagnitude <= 0.25f)
                FinishCast(false, "Pancing kembali. Lempar lagi ke kolam.");
        }

        private Vector3 ShorePoint()
        {
            Vector3 bank = player.transform.position;
            bank.y = water.SurfaceY - tuning.HookDepth;
            if (landingBank != null)
            {
                Bounds dock = landingBank.bounds;
                Vector3 intoPond = water.Bounds.center - dock.center;
                bool edgeAlongX = Mathf.Abs(intoPond.z) >= Mathf.Abs(intoPond.x);
                const float clearance = 0.22f;
                if (edgeAlongX && bank.x >= dock.min.x && bank.x <= dock.max.x)
                    bank.z = intoPond.z < 0 ? dock.min.z - clearance : dock.max.z + clearance;
                else if (!edgeAlongX && bank.z >= dock.min.z && bank.z <= dock.max.z)
                    bank.x = intoPond.x < 0 ? dock.min.x - clearance : dock.max.x + clearance;
            }

            return water.KeepInWater(bank, 0.12f);
        }

        private void FightFish(float deltaTime, bool reel)
        {
            var profile = fish.FightTuning;
            Vector3 tip = rodTip.position;
            Vector3 shore = ShorePoint();
            if (!lifting && reel && !fight.IsRunning &&
                Vector3.ProjectOnPlane(fish.Position - shore, Vector3.up).sqrMagnitude <=
                    tuning.ShoreLiftRadius * tuning.ShoreLiftRadius)
            {
                lifting = true;
                liftPoint = shore;
            }

            if (lifting)
            {
                if (fish.Position.y >= water.SurfaceY + tuning.BankClearance) clearedBank = true;
                Vector3 liftTarget = clearedBank ? tip : new Vector3(liftPoint.x, tip.y, liftPoint.z);
                if (reel)
                    fightAnchor = Vector3.MoveTowards(fightAnchor, liftTarget, tuning.LiftSpeed * deltaTime);
            }
            else fightAnchor = shore;

            float distance = Vector3.Distance(fish.Position, fightAnchor);
            float reelSpeed = tuning.ReelSpeed * (lifting ? tuning.LiftReelFactor : 1f);
            fight.Advance(deltaTime, reel, distance, reelSpeed);
            if (fight.HasSnapped)
            {
                FinishCast(false, "Tali putus. Ikan lepas.");
                return;
            }

            fish.ConstrainLine(fightAnchor, fight.LineLength);
            fish.SetRun(fight.IsRunning, fight.RunDirection);
            bobber.SetTether(fish.Position + Vector3.up * tuning.HookDepth);
            rodVisual.transform.localRotation = restRodRotation *
                Quaternion.Euler(fight.Tension * profile.RodDipDegrees, 0, 0);

            if (clearedBank && !fight.IsRunning && reel &&
                Vector3.Distance(tip, fish.Position) <= profile.LandingDistance + 0.01f)
                FinishCast(true, null);
        }

        private void FinishCast(bool caught, string message)
        {
            var item = caught ? fish.CatchItem : null;
            if (caught && (!household.IsDayRunning || !catchBag.TryAdd(item, 1)))
            {
                caught = false;
                message = "Tangkapan tidak bisa disimpan.";
            }

            household.EndFishingCast(caught);
            if (caught)
            {
                fish.Land();
                fish = null;
                household.ReportActivity($"{item.DisplayName} masuk tas • {catchBag.Count(item)} tersimpan.");
            }
            else
            {

                ReleaseFish();
                household.ReportActivity(message);
            }

            ClearFight();
            SetLineVisible(false);
            SetPhase(Phase.Ready);
            equippedFrame = Time.frameCount;
        }

        private void ReleaseFish()
        {
            if (fish != null) fish.Release();
            fish = null;
        }

        private void ClearFight()
        {
            fight = null;
            lifting = clearedBank = false;
            rodVisual.transform.localRotation = restRodRotation;
        }

        private void SetPhase(Phase next)
        {
            phase = next;
            phaseTime = 0;
        }

        private void SetLineVisible(bool visible)
        {
            floatVisual.gameObject.SetActive(visible);
            line.enabled = visible;
        }

        private void UpdateLine()
        {
            line.SetPosition(0, rodTip.position);
            line.SetPosition(1, phase == Phase.Hooked ? fish.Position : bobber.Position);
        }

        private void PutAway()
        {
            if (phase == Phase.Idle) return;

            household.EndFishingCast(false);
            ReleaseFish();
            ClearFight();
            SetPhase(Phase.Idle);
            ownedCast?.Disable();
            ownedExit?.Disable();
            ownedCast = ownedExit = null;
            player.SetWorldInteractionBlocked(false);
            SetLineVisible(false);
            rodVisual.SetActive(false);
        }

        private void OnDisable() => PutAway();
    }
}
