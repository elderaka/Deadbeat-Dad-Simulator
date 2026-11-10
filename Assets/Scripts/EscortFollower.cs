using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace BapakSimulator
{
    public sealed class EscortFollower : VehiclePassenger
    {
        [SerializeField] private Transform leader;
        [SerializeField] private Transform home;
        [SerializeField] private Transform destination;
        [SerializeField, Min(0.5f)] private float arrivalRadius = 2.5f;
        [SerializeField, Min(0.05f)] private float repathInterval = 0.2f;

        private ResidentEscortDialogue dialogue;

        [SerializeField] private UnityEvent arrivedDestination = new UnityEvent();
        [SerializeField] private UnityEvent arrivedHome = new UnityEvent();
        internal event System.Action<Transform> EscortedArrival;

        internal void SyncRoutineArrival(Transform point)
        {
            if (point == home) nextStop = destination;
            else if (point == destination) nextStop = home;
        }

        public UnityEvent ArrivedDestination => arrivedDestination;
        public UnityEvent ArrivedHome => arrivedHome;

        private Transform nextStop;
        private bool following;
        private float nextRepath;
        private string recruitPrompt;

        public bool IsFollowing => following;
        public bool IsReturningHome => nextStop == home;
        protected override bool WantsRide => following;
        public Transform NextStop => nextStop;
        protected override Transform ParkingDestination => following ? nextStop : null;

        public override string Prompt => following ? recruitPrompt + " • Ikut" : recruitPrompt;

        protected override void Awake()
        {
            base.Awake();
            recruitPrompt = base.Prompt;
            if (leader == null || home == null || destination == null || home == destination)
            {
                Debug.LogError($"{nameof(EscortFollower)} on '{name}' needs a leader and two distinct stops.", this);
                enabled = false;
                return;
            }

            dialogue = GetComponent<ResidentEscortDialogue>();
            nextStop = destination;
        }

        public override bool TryInteract()
        {
            if (!isActiveAndEnabled || following || !Agent.isOnNavMesh) return false;

            return dialogue != null ? dialogue.TryTalk() : TryStartFollowing();
        }

        internal bool TryStartFollowing()
        {
            if (!isActiveAndEnabled || following || !Agent.isOnNavMesh) return false;

            Agent.ResetPath();
            following = true;
            nextRepath = 0;
            return true;
        }

        protected override void OnDisembarked(bool completedStop) => nextRepath = 0;

        protected override void OnParkingArrival() => CompleteLeg();

        private void CompleteLeg()
        {
            Agent.ResetPath();
            following = false;
            var reached = nextStop;
            bool returnedHome = reached == home;
            nextStop = returnedHome ? destination : home;
            if (returnedHome) arrivedHome.Invoke();
            else arrivedDestination.Invoke();
            EscortedArrival?.Invoke(reached);
        }

        protected override void Update()
        {
            base.Update();
            if (!following || IsRiding || IsWalkingFromParking || !Agent.isOnNavMesh || Time.time < nextRepath) return;

            nextRepath = Time.time + repathInterval;
            bool leaderAtStop = (leader.position - nextStop.position).sqrMagnitude <= arrivalRadius * arrivalRadius;
            if (leaderAtStop && (transform.position - nextStop.position).sqrMagnitude <=
                Agent.stoppingDistance * Agent.stoppingDistance)
            {
                CompleteLeg();
                return;
            }

            // A jumping leader is projected back to the walkable surface.
            Vector3 target = leaderAtStop ? nextStop.position : leader.position;
            if (NavMesh.SamplePosition(target, out NavMeshHit hit, arrivalRadius, Agent.areaMask))
                Agent.SetDestination(hit.position);
        }
    }
}
