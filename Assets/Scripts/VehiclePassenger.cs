using UnityEngine;
using UnityEngine.AI;

namespace BapakSimulator
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class VehiclePassenger : WorldInteractable
    {
        private NavMeshAgent agent;
        private Transform walkingParent;
        private Vector3 boardingPosition;
        private Collider boardingCollider;
        private bool colliderWasEnabled;

        private Transform parkingDestination;
        private Vector3 parkingArrival;

        protected bool IsWalkingFromParking => parkingDestination != null;
        protected virtual Transform ParkingDestination => null;
        protected NavMeshAgent Agent => agent;
        public bool IsRiding { get; private set; }
        protected abstract bool WantsRide { get; }
        public bool CanBoard => isActiveAndEnabled && !IsRiding && !IsWalkingFromParking && WantsRide &&
            agent != null && agent.enabled && agent.isOnNavMesh;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            boardingCollider = GetComponent<Collider>();
        }

        internal virtual bool CanBoardAt(ParkingMarker marker) => true;

        internal bool TryBoard(Transform seat)
        {
            if (!CanBoard || seat == null) return false;

            walkingParent = transform.parent;
            boardingPosition = transform.position;
            if (boardingCollider != null)
            {
                colliderWasEnabled = boardingCollider.enabled;
                boardingCollider.enabled = false;
            }

            agent.ResetPath();
            agent.enabled = false;
            transform.SetParent(seat, false);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            IsRiding = true;
            OnBoarded();
            return true;
        }

        internal bool TryFindExit(Vector3 position, out Vector3 exit)
        {
            exit = position;
            if (!IsRiding || !NavMesh.SamplePosition(position, out NavMeshHit hit, 0.6f, agent.areaMask))
                return false;

            exit = hit.position;
            return true;
        }

        internal void Disembark(Vector3 position, bool completedStop)
        {
            transform.SetParent(walkingParent, true);
            transform.position = position;
            agent.enabled = true;
            if (boardingCollider != null) boardingCollider.enabled = colliderWasEnabled;
            IsRiding = false;
            OnDisembarked(completedStop);
        }

        internal bool TryDropAt(ParkingMarker marker, Vector3 exitPosition)
        {
            var destination = ParkingDestination;
            if (!marker.Accepts(destination) || !TryFindExit(exitPosition, out var exit)) return false;

            var filter = new NavMeshQueryFilter { agentTypeID = agent.agentTypeID, areaMask = agent.areaMask };
            var path = new NavMeshPath();
            if (!NavMesh.SamplePosition(destination.position, out var arrival, 1f, filter) ||
                !NavMesh.CalculatePath(exit, arrival.position, filter, path) || path.status != NavMeshPathStatus.PathComplete)
                return false;

            parkingDestination = destination;
            parkingArrival = arrival.position;
            Disembark(exit, false);
            if (agent.SetPath(path)) return true;

            // The exit and route were validated. Retry navigation from the new position if the agent rejected the cached path.
            agent.SetDestination(arrival.position);
            return true;
        }

        protected virtual void Update()
        {
            if (!IsWalkingFromParking || agent.pathPending || !agent.isOnNavMesh ||
                agent.pathStatus != NavMeshPathStatus.PathComplete || agent.remainingDistance > agent.stoppingDistance ||
                (agent.nextPosition - parkingArrival).sqrMagnitude > Mathf.Pow(agent.stoppingDistance + .1f, 2))
                return;

            parkingDestination = null;
            agent.ResetPath();
            OnParkingArrival();
        }

        protected virtual void OnParkingArrival() { }

        internal void AbortRide() => Disembark(boardingPosition, false);

        protected virtual void OnBoarded() { }
        protected virtual void OnDisembarked(bool completedStop) { }
    }
}
