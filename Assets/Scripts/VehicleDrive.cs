using UnityEngine;
using UnityEngine.InputSystem;

namespace BapakSimulator
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class VehicleDrive : WorldInteractable
    {
        [SerializeField] private VehicleHandling handling;
        [SerializeField] private PlayerController player;
        [System.Serializable]
        private sealed class PassengerSeat
        {
            public Transform seat;
            public Transform exit;
            [System.NonSerialized] public VehiclePassenger occupant;
            [System.NonSerialized] public Vector3 exitPosition;
        }

        [SerializeField] private PassengerSeat[] passengerSeats = System.Array.Empty<PassengerSeat>();
        [SerializeField] private string persistentId;
        [SerializeField] private bool initiallyUnlocked = true;
        [SerializeField, Range(0, 1)] private float startingFuelFraction = 1f;
        [SerializeField] private Transform driverSeat;
        [SerializeField] private Transform driverExit;
        [SerializeField] private Transform driveCamera;
        [SerializeField] private Transform chasePivot;
        [SerializeField, Min(0.1f)] private float chaseDistance;
        [SerializeField, Range(-15, 75)] private float chasePitch;
        [SerializeField] private Transform visual;
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference exitAction;
        [SerializeField, Min(0.1f)] private float boardingDistance = 5f;
        [SerializeField, Min(0.05f)] private float boardingCheckInterval = 0.25f;
        [SerializeField, Min(0.1f)] private float stoppedSpeedKph = 2f;
        [SerializeField] private LayerMask passengerLayers = Physics.DefaultRaycastLayers;

        private Rigidbody body;
        private Collider[] nearbyContacts = new Collider[16];
        private float nextBoardingCheck;
        private float nextParkingCheck;
        private ParkingMarker parkedAt;
        private string exitHint;
        private InputAction ownedMove;
        private InputAction ownedExit;
        private Vector2 input;
        private int mountedFrame;
        private bool driving;
        private Quaternion neutralVisualRotation;

        private Vector3 odometerPosition;

        internal string PersistentId => persistentId;
        public bool IsUnlocked { get; private set; }
        public double OdometerMeters { get; private set; }
        public double FuelUsedLiters { get; private set; }
        public double FuelPurchasedLiters { get; private set; }
        public long FuelSpent { get; private set; }

        public void Unlock() => IsUnlocked = true;

        internal Transform DriverCameraMount => driveCamera;
        internal Transform ChasePivot => chasePivot;
        internal float ChaseDistance => chaseDistance;
        internal float ChasePitch => chasePitch;
        internal bool CanUseChaseView => chasePivot != null && chaseDistance > 0;

        public float SpeedKph => Vector3.ProjectOnPlane(body.linearVelocity, Vector3.up).magnitude * 3.6f;
        public float FuelLiters { get; private set; }
        public float FuelCapacity => handling.FuelCapacity;
        public bool CanRefuel => isActiveAndEnabled && IsUnlocked && !driving && IsStopped;
        private bool IsStopped => SpeedKph <= stoppedSpeedKph;

        internal void RecordFuelPurchase(float liters, int cost)
        {
            float previousFuel = FuelLiters;
            FuelLiters = Mathf.Min(FuelCapacity, FuelLiters + liters);
            FuelPurchasedLiters += FuelLiters - previousFuel;
            FuelSpent = FuelSpent > long.MaxValue - cost ? long.MaxValue : FuelSpent + cost;
        }

        internal CheckpointData.VehicleData CaptureCheckpoint() => new CheckpointData.VehicleData
        {
            id = persistentId,
            unlocked = IsUnlocked,
            tankLiters = FuelLiters,
            odometerMeters = OdometerMeters,
            fuelUsedLiters = FuelUsedLiters,
            fuelPurchasedLiters = FuelPurchasedLiters,
            fuelSpent = FuelSpent
        };

        internal void RestoreCheckpoint(CheckpointData.VehicleData data)
        {
            IsUnlocked = data.unlocked;
            FuelLiters = Mathf.Min(FuelCapacity, data.tankLiters);
            OdometerMeters = data.odometerMeters;
            FuelUsedLiters = data.fuelUsedLiters;
            FuelPurchasedLiters = data.fuelPurchasedLiters;
            FuelSpent = data.fuelSpent;
        }
        public int EstimatedRpm => FuelLiters <= 0 ? 0 : Mathf.RoundToInt(Mathf.Lerp(
            handling.IdleRpm, handling.MaximumRpm, SpeedKph / (3.6f * handling.ForwardSpeed)));
        public string Status { get; private set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            if (handling == null || !handling.IsValid || player == null || driverSeat == null ||
                driverExit == null || driveCamera == null || visual == null ||
                moveAction?.action == null || exitAction?.action == null ||
                boardingDistance <= 0 || boardingCheckInterval <= 0 || stoppedSpeedKph <= 0 ||
                System.Array.Exists(passengerSeats, seat => seat == null || seat.seat == null || seat.exit == null))
            {
                Debug.LogError($"{nameof(VehicleDrive)} on '{name}' has incomplete vehicle setup.", this);
                enabled = false;
                return;
            }

            IsUnlocked = initiallyUnlocked;
            FuelLiters = handling.FuelCapacity * startingFuelFraction;
            neutralVisualRotation = visual.localRotation;
        }

        public override bool TryInteract()
        {
            if (!isActiveAndEnabled || !IsUnlocked || driving || !IsStopped ||
                !player.BeginDriving(this, driverSeat))
                return false;

            driving = true;
            odometerPosition = body.position;
            mountedFrame = Time.frameCount;
            ownedMove = EnableIfNeeded(moveAction.action);
            ownedExit = EnableIfNeeded(exitAction.action);
            string exitBinding = exitAction.action.GetBindingDisplayString(
                InputBinding.DisplayStringOptions.DontIncludeInteractions, "Keyboard&Mouse");
            exitHint = string.IsNullOrWhiteSpace(exitBinding) ? "Turun saat berhenti" : $"{exitBinding}: turun saat berhenti";
            Status = exitHint;
            nextBoardingCheck = 0;
            TryBoardPassengers();

            return true;
        }

        private static InputAction EnableIfNeeded(InputAction action)
        {
            if (action.enabled) return null;
            action.Enable();
            return action;
        }

        private void TryBoardPassengers()
        {
            if (!IsStopped || Time.time < nextBoardingCheck) return;
            nextBoardingCheck = Time.time + boardingCheckInterval;

            bool boarded = false;
            int contactCount = -1;
            foreach (var seat in passengerSeats)
            {
                if (seat.occupant != null) continue;
                if (contactCount < 0)
                    contactCount = CollectNearbyPassengers();

                VehiclePassenger nearest = null;
                float nearestDistance = boardingDistance * boardingDistance;
                for (int i = 0; i < contactCount; i++)
                {
                    var candidate = nearbyContacts[i].GetComponentInParent<VehiclePassenger>();
                    if (candidate == null || !candidate.CanBoard || !candidate.CanBoardAt(parkedAt)) continue;

                    float distance = (candidate.transform.position - seat.seat.position).sqrMagnitude;
                    if (distance > nearestDistance) continue;

                    nearestDistance = distance;
                    nearest = candidate;
                }

                if (nearest == null || !nearest.TryBoard(seat.seat)) continue;

                seat.occupant = nearest;
                boarded = true;
            }

            if (boarded) RefreshPassengerStatus();
        }

        private void RefreshPassengerStatus()
        {
            var names = new System.Collections.Generic.List<string>();
            foreach (var seat in passengerSeats)
                if (seat.occupant != null) names.Add(seat.occupant.name);

            Status = names.Count > 0 ? $"{string.Join(", ", names)} ikut • {exitHint}" : exitHint;
        }

        private int CollectNearbyPassengers()
        {
            // A larger contact buffer keeps boarding reliable in dense scenes.
            while (true)
            {
                int count = Physics.OverlapSphereNonAlloc(transform.position, boardingDistance,
                    nearbyContacts, passengerLayers, QueryTriggerInteraction.Ignore);
                if (count < nearbyContacts.Length) return count;

                System.Array.Resize(ref nearbyContacts, nearbyContacts.Length * 2);
            }
        }

        private void Update()
        {
            if (player.IsPaused) return;
            UpdateParking();
            if (!driving) return;

            TryBoardPassengers();
            input = Application.isFocused ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
            if (Time.frameCount > mountedFrame && exitAction.action.WasPressedThisFrame())
                TryExit();
        }

        private void UpdateParking()
        {
            if (parkedAt != null && !parkedAt.ContainsStoppedVehicle(this))
            {
                parkedAt.SetParked(this, false);
                parkedAt = null;
            }

            if (!IsStopped || Time.time < nextParkingCheck) return;
            nextParkingCheck = Time.time + boardingCheckInterval;
            if (parkedAt == null)
            {
                int count = Physics.OverlapSphereNonAlloc(transform.position, .05f, nearbyContacts,
                    Physics.AllLayers, QueryTriggerInteraction.Collide);
                for (int i = 0; i < count; i++)
                {
                    var marker = nearbyContacts[i].GetComponent<ParkingMarker>();
                    if (marker == null || !marker.ContainsStoppedVehicle(this)) continue;
                    parkedAt = marker;
                    marker.SetParked(this, true);
                    break;
                }
            }

            if (parkedAt == null) return;
            bool dropped = false;
            foreach (var seat in passengerSeats)
            {
                if (seat.occupant == null || !seat.occupant.TryDropAt(parkedAt, seat.exit.position)) continue;
                seat.occupant = null;
                dropped = true;
            }
            if (dropped && driving) RefreshPassengerStatus();
        }

        public bool TryExit()
        {
            if (!driving) return false;
            if (!IsStopped)
            {
                Status = "Berhenti dulu untuk turun.";
                return false;
            }

            if (!player.CanExitVehicle(driverExit.position))
            {
                Status = "Tidak ada ruang aman untuk turun.";
                return false;
            }

            foreach (var seat in passengerSeats)
            {
                if (seat.occupant == null ||
                    seat.occupant.TryFindExit(seat.exit.position, out seat.exitPosition)) continue;

                Status = "Tidak ada ruang aman untuk penumpang turun.";
                return false;
            }

            player.EndDriving(driverExit.position, body.rotation);
            foreach (var seat in passengerSeats)
            {
                if (seat.occupant == null) continue;

                seat.occupant.Disembark(seat.exitPosition, true);
                seat.occupant = null;
            }

            StopDriving();
            return true;
        }

        private void StopDriving()
        {
            driving = false;
            input = Vector2.zero;
            ownedMove?.Disable();
            ownedExit?.Disable();
            ownedMove = ownedExit = null;
        }

        private void OnDisable()
        {
            if (parkedAt != null) parkedAt.SetParked(this, false);
            parkedAt = null;
            if (driving && player != null && player.ActiveVehicle == this)
            {
                foreach (var seat in passengerSeats)
                {
                    if (seat.occupant == null) continue;
                    if (seat.occupant.TryFindExit(seat.exit.position, out var exit))
                        seat.occupant.Disembark(exit, false);
                    else
                        seat.occupant.AbortRide();
                    seat.occupant = null;
                }
                player.EndDriving(driverExit.position, body.rotation);
            }
            StopDriving();
        }

        private void FixedUpdate() => StepDrive(input, Time.fixedDeltaTime);

        private void StepDrive(Vector2 controls, float deltaTime)
        {
            if (driving)
            {
                OdometerMeters += Vector3.ProjectOnPlane(body.position - odometerPosition, Vector3.up).magnitude;
                odometerPosition = body.position;
            }

            body.angularVelocity = Vector3.zero;
            Vector3 velocity = body.linearVelocity;
            Vector3 forward = body.rotation * Vector3.forward;
            float forwardSpeed = Vector3.Dot(velocity, forward);
            bool grounded = Physics.Raycast(body.position + Vector3.up * 0.2f, Vector3.down, 0.55f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            if (!grounded) return;

            float throttle = driving && FuelLiters > 0 ? controls.y : 0;
            bool opposing = Mathf.Abs(forwardSpeed) > 0.3f && forwardSpeed * throttle < 0;
            float targetSpeed = opposing ? 0 : throttle >= 0 ? throttle * handling.ForwardSpeed :
                throttle * handling.ReverseSpeed;
            float rate = opposing || Mathf.Abs(targetSpeed) < Mathf.Abs(forwardSpeed)
                ? handling.Braking : handling.Acceleration;
            float nextSpeed = Mathf.MoveTowards(forwardSpeed, targetSpeed, rate * deltaTime);

            float steering = driving ? controls.x : 0;
            float steeringScale = Mathf.Clamp01(Mathf.Abs(nextSpeed) / 2f);
            float yaw = steering * handling.SteeringRate * steeringScale * Mathf.Sign(nextSpeed) * deltaTime;
            Quaternion rotation = Quaternion.AngleAxis(yaw, Vector3.up) * body.rotation;
            if (yaw != 0) body.MoveRotation(rotation);
            body.linearVelocity = rotation * Vector3.forward * nextSpeed + Vector3.up * velocity.y;
            visual.localRotation = Quaternion.Slerp(visual.localRotation,
                neutralVisualRotation * Quaternion.Euler(0, 0, -steering * steeringScale * handling.LeanDegrees),
                Mathf.Clamp01(deltaTime * 8f));

            if (driving && throttle * nextSpeed > 0)
            {
                float used = Mathf.Min(FuelLiters, Mathf.Abs(nextSpeed) * deltaTime *
                    handling.LitersPerKilometer / 1000f);
                FuelLiters -= used;
                FuelUsedLiters += used;
            }
            if (driving && FuelLiters <= 0)
                Status = $"Bensin habis • {exitHint}";
        }
    }
}
