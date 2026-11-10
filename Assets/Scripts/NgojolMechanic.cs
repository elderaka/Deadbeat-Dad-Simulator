using System;
using UnityEngine;
using UnityEngine.AI;

namespace BapakSimulator
{
    public sealed class NgojolMechanic : VehiclePassenger
    {
        [Serializable]
        private sealed class Route
        {
            public string pickupLabel;
            public ParkingMarker pickupBay;
            public Transform pickupPoint;
            public string destinationLabel;
            public ParkingMarker dropOffBay;
            public Transform dropOff;
            [Min(1)] public int fare;

            public bool IsValid => !string.IsNullOrWhiteSpace(pickupLabel) && pickupBay != null && pickupPoint != null &&
                !string.IsNullOrWhiteSpace(destinationLabel) && dropOffBay != null && dropOff != null &&
                pickupBay != dropOffBay && dropOffBay.Accepts(dropOff) && fare > 0;
        }

        [SerializeField] private PickupDay household;
        [SerializeField] private NeighborhoodDialogue dialogue;
        [SerializeField] private string dialogueNode;
        [SerializeField] private Route[] routes = Array.Empty<Route>();

        private enum JobState { Offered, Accepted, Complete }
        private JobState state;
        private bool hasBoarded;
        private int routeIndex;
        private NpcResident resident;
        private NeighborBuyout buyout;

        public string RiderName => resident != null ? resident.Profile.DisplayName : base.Prompt;

        private Route Current => routes[routeIndex];
        public bool HasActiveOrder => state == JobState.Accepted;
        public bool CanOpenOffer => isActiveAndEnabled && !HasActiveOrder;
        public string PhoneStatus
        {
            get
            {
                if (!isActiveAndEnabled) return string.Empty;
                var route = Current;
                return state switch
                {
                    JobState.Accepted when !hasBoarded => $"Jemput: {route.pickupLabel}\nTujuan: {route.destinationLabel}",
                    JobState.Accepted => $"Antar ke {route.destinationLabel}",
                    JobState.Complete => $"Order selesai • +Rp{route.fare:N0}",
                    _ => $"{route.pickupLabel} → {route.destinationLabel}\nRp{route.fare:N0}"
                };
            }
        }

        protected override bool WantsRide => HasActiveOrder;
        protected override Transform ParkingDestination => HasActiveOrder ? Current.dropOff : null;
        public override string Prompt => state switch
        {
            JobState.Accepted => $"{base.Prompt} • {Current.destinationLabel}",
            JobState.Complete => $"{base.Prompt} • Selesai",
            _ => base.Prompt
        };

        protected override void Awake()
        {
            base.Awake();
            resident = GetComponent<NpcResident>();
            buyout = GetComponent<NeighborBuyout>();
            if (household != null && dialogue != null && !string.IsNullOrWhiteSpace(dialogueNode) &&
                routes.Length > 0 && !Array.Exists(routes, route => route == null || !route.IsValid))
                return;
            Debug.LogError($"Ngojol on '{name}' needs a dialogue and valid routes with pickup and drop-off bays.", this);
            enabled = false;
        }

        public void OpenPhoneOffer()
        {
            if (!CanOpenOffer || !household.IsConfigured || !Agent.isOnNavMesh) return;
            if (state == JobState.Complete && !TryAdvanceRoute()) return;
            StartConversation(true);
        }

        private bool TryAdvanceRoute()
        {
            int next = (routeIndex + 1) % routes.Length;
            var point = routes[next].pickupPoint;
            var filter = new NavMeshQueryFilter { agentTypeID = Agent.agentTypeID, areaMask = Agent.areaMask };
            if (!NavMesh.SamplePosition(point.position, out var spawn, 1f, filter) || !Agent.Warp(spawn.position))
                return false;

            Agent.ResetPath();
            routeIndex = next;
            state = JobState.Offered;
            hasBoarded = false;
            return true;
        }

        public override bool TryInteract() => StartConversation(false);

        private bool StartConversation(bool byPhone)
        {
            if (!isActiveAndEnabled || !household.IsConfigured || IsRiding || IsWalkingFromParking || !Agent.isOnNavMesh)
                return false;

            var route = Current;
            return dialogue.TryStart(dialogueNode, variables =>
            {
                if (!byPhone && buyout != null) buyout.Prepare(variables);
                else
                {
                    variables.SetValue("$buyout_disclose", false);
                    variables.SetValue("$buyout_available", false);
                    variables.SetValue("$buyout_declined", false);
                    variables.SetValue("$buyout_sold", false);
                }
                variables.SetValue("$ride_rider", RiderName);
                variables.SetValue("$ride_pickup", route.pickupLabel);
                variables.SetValue("$ride_destination", route.destinationLabel);
                variables.SetValue("$ride_fare", route.fare.ToString("N0"));
                variables.SetValue("$ride_offered", state == JobState.Offered);
                variables.SetValue("$ride_complete", state == JobState.Complete);
            }, action => action == "accept_ride" ? route == Current && AcceptOrder() :
                !byPhone && buyout != null && buyout.Execute(action));
        }

        private bool AcceptOrder()
        {
            if (!isActiveAndEnabled || !household.IsConfigured || state != JobState.Offered || !Agent.isOnNavMesh)
                return false;

            state = JobState.Accepted;
            Current.pickupBay.SetRouteVisible(true);
            household.ReportActivity($"Jemput penumpang di {Current.pickupLabel}.");
            return true;
        }

        internal override bool CanBoardAt(ParkingMarker marker) => hasBoarded || marker == Current.pickupBay;

        protected override void OnBoarded()
        {
            hasBoarded = true;
            Current.pickupBay.SetRouteVisible(false);
            Current.dropOffBay.SetRouteVisible(true);
            household.ReportActivity($"Antar penumpang ke {Current.destinationLabel}.");
        }

        protected override void OnDisembarked(bool completedStop)
        {
            if (HasActiveOrder && completedStop)
                household.ReportActivity($"Parkir di marka {Current.destinationLabel} untuk mengantar penumpang.");
        }

        protected override void OnParkingArrival()
        {
            if (!HasActiveOrder) return;
            state = JobState.Complete;
            Current.dropOffBay.SetRouteVisible(false);
            household.Progression.CreditCash(Current.fare);
            household.ReportActivity($"Order selesai • +Rp{Current.fare}.");
            if (buyout != null && buyout.Hangout != null)
            {
                var filter = new NavMeshQueryFilter { agentTypeID = Agent.agentTypeID, areaMask = Agent.areaMask };
                if (NavMesh.SamplePosition(buyout.Hangout.position, out var point, 1f, filter))
                    Agent.SetDestination(point.position);
            }
        }

        private void OnDisable()
        {
            if (!HasActiveOrder) return;
            Current.pickupBay.SetRouteVisible(false);
            Current.dropOffBay.SetRouteVisible(false);
        }
    }
}
