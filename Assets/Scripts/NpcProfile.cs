using System;
using UnityEngine;

namespace BapakSimulator
{
    [CreateAssetMenu(menuName = "Bapak Simulator/NPC/Resident")]
    public sealed class NpcProfile : ScriptableObject
    {
        [Serializable]
        public struct RoutineEntry
        {
            public string activity;
            [Range(0, 1440)] public int startMinute;
            [Range(0, 1440)] public int endMinute;
            public bool blocksRequests;
            public NpcRoutineLocation location;
            public bool waitForSchoolPickup;
        }

        [SerializeField] private RelationshipDefinition identity;
        [SerializeField] private string displayName;
        [SerializeField] private RoutineEntry[] routine = Array.Empty<RoutineEntry>();

        internal System.Collections.Generic.IEnumerable<RoutineEntry> Routine => routine;
        public string DisplayName => displayName;
        internal RelationshipDefinition Identity => identity;

        internal bool IsValid
        {
            get
            {
                if (identity == null || string.IsNullOrWhiteSpace(identity.PersistentId) ||
                    string.IsNullOrWhiteSpace(displayName)) return false;
                int previousEnd = 0;
                foreach (var entry in routine)
                {
                    if (string.IsNullOrWhiteSpace(entry.activity) || entry.startMinute < previousEnd ||
                        entry.endMinute <= entry.startMinute || entry.endMinute > 24 * 60) return false;
                    previousEnd = entry.endMinute;
                }
                return true;
            }
        }

        public bool TryGetActivity(int minute, out RoutineEntry activity)
        {
            foreach (var entry in routine)
            {
                if (minute < entry.startMinute) break;
                if (minute >= entry.endMinute) continue;
                activity = entry;
                return true;
            }
            activity = default;
            return false;
        }

        public bool HasAvailability(float earliest, int deadline, int duration)
        {
            if (duration <= 0 || earliest < 0 || deadline > 24 * 60) return false;
            foreach (var entry in routine)
            {
                if (!entry.blocksRequests || entry.endMinute <= earliest) continue;
                if (earliest + duration <= entry.startMinute) break;
                earliest = entry.endMinute;
            }
            return earliest + duration <= deadline;
        }

    }
}
