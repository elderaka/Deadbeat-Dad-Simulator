using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace BapakSimulator
{
    [DisallowMultipleComponent, RequireComponent(typeof(NpcResident), typeof(NavMeshAgent))]
    public sealed class NpcRoutineWalker : MonoBehaviour
    {
        [Serializable]
        private sealed class LocationBinding
        {
            public NpcRoutineLocation location;
            public Transform point;
            public UnityEvent arrived;
        }

        [SerializeField] private LocationBinding[] locations = Array.Empty<LocationBinding>();
        [SerializeField, Min(.05f)] private float retryInterval = .5f;
        [SerializeField, Min(.05f)] private float sampleRadius = 1f;

        private readonly Dictionary<NpcRoutineLocation, LocationBinding> byLocation = new();
        private NpcResident resident;
        private NavMeshAgent agent;
        private EscortFollower escort;
        private LocationBinding walkingTo;
        private LocationBinding reached;
        private int observedDay;
        private int resumeRoutineMinute;
        private float nextAttempt;
        private Vector3 arrivalPoint;

        private void Awake()
        {
            resident = GetComponent<NpcResident>();
            agent = GetComponent<NavMeshAgent>();
            escort = GetComponent<EscortFollower>();
            foreach (var binding in locations)
            {
                if (binding != null && binding.location != null && binding.point != null &&
                    byLocation.TryAdd(binding.location, binding)) continue;
                RejectConfiguration();
                return;
            }

            if (!resident.IsConfigured || retryInterval <= 0 || sampleRadius <= 0)
            {
                RejectConfiguration();
                return;
            }

            foreach (var entry in resident.Profile.Routine)
            {
                if (entry.location == null || byLocation.ContainsKey(entry.location)) continue;
                RejectConfiguration();
                return;
            }
        }

        private void RejectConfiguration()
        {
            Debug.LogError($"Routine Walker on '{name}' needs one scene point for each authored routine location.", this);
            enabled = false;
        }

        private void OnEnable()
        {
            if (escort != null) escort.EscortedArrival += OnEscortedArrival;
        }

        private void OnDisable()
        {
            if (escort != null) escort.EscortedArrival -= OnEscortedArrival;
            ReleasePath();
        }

        private void OnEscortedArrival(Transform point)
        {
            walkingTo = null;
            reached = null;
            observedDay = resident.DayNumber;
            if (!resident.TryGetCurrentActivity(out var activeEntry)) return;
            resumeRoutineMinute = activeEntry.endMinute;
            if (activeEntry.location != null && byLocation[activeEntry.location].point == point)
            {
                resumeRoutineMinute = activeEntry.startMinute;
                return;
            }
            foreach (var entry in resident.Profile.Routine)
            {
                if (entry.startMinute < activeEntry.endMinute || entry.location == null ||
                    byLocation[entry.location].point != point) continue;
                resumeRoutineMinute = entry.startMinute;
                break;
            }
        }

        private void Update()
        {
            if (!CanOwnPath() || !agent.enabled || !agent.isOnNavMesh)
            {
                walkingTo = null;
                return;
            }

            int day = resident.DayNumber;
            if (observedDay != day)
            {
                observedDay = day;
                reached = null;
                resumeRoutineMinute = 0;
            }

            if (!resident.TryGetTravelActivity(out var activity) || activity.location == null ||
                activity.startMinute < resumeRoutineMinute || resident.IsTalkingInPerson || resident.HoldRoutineTravel)
            {
                ReleasePath();
                return;
            }

            var target = byLocation[activity.location];
            if (target == reached) return;

            if (walkingTo != null && walkingTo != target) ReleasePath();

            if (walkingTo == null)
            {
                if (Time.time < nextAttempt) return;
                nextAttempt = Time.time + retryInterval;
                var filter = new NavMeshQueryFilter { agentTypeID = agent.agentTypeID, areaMask = agent.areaMask };
                if (!NavMesh.SamplePosition(target.point.position, out var sample, sampleRadius, filter) ||
                    !agent.SetDestination(sample.position)) return;
                walkingTo = target;
                arrivalPoint = sample.position;
            }

            if (agent.pathPending) return;
            float tolerance = agent.stoppingDistance + .15f;
            if ((agent.nextPosition - arrivalPoint).sqrMagnitude <= tolerance * tolerance)
            {
                reached = target;
                ReleasePath();
                if (escort != null) escort.SyncRoutineArrival(target.point);
                target.arrived?.Invoke();
            }
            else if (agent.pathStatus != NavMeshPathStatus.PathComplete || !agent.hasPath)
                ReleasePath();
        }

        private bool CanOwnPath() => escort == null || (!escort.IsFollowing && !escort.IsRiding);

        private void ReleasePath()
        {
            if (walkingTo == null) return;
            walkingTo = null;
            if (CanOwnPath() && agent != null && agent.enabled && agent.isOnNavMesh) agent.ResetPath();
        }
    }
}
