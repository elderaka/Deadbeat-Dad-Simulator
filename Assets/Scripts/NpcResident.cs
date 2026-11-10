using UnityEngine;

namespace BapakSimulator
{
    [DisallowMultipleComponent]
    public sealed class NpcResident : MonoBehaviour
    {
        [SerializeField] private NpcProfile profile;
        [SerializeField] private PickupDay household;

        public NpcProfile Profile => profile;
        internal bool HoldRoutineTravel { get; set; }
        internal bool IsTalkingInPerson { get; set; }
        internal int DayNumber => household.DayNumber;

        internal bool TryGetTravelActivity(out NpcProfile.RoutineEntry activity) =>
            TryGetCurrentActivity(out activity) && (!activity.waitForSchoolPickup || household.IsSchoolPickupResolved);
        public bool IsConfigured => household != null && profile != null && profile.IsValid;
        public bool TryGetCurrentActivity(out NpcProfile.RoutineEntry activity)
        {
            if (IsConfigured && household.IsDayRunning)
                return profile.TryGetActivity(household.Minute, out activity);
            activity = default;
            return false;
        }

        private void Awake()
        {
            if (IsConfigured) return;
            Debug.LogError($"Resident '{name}' needs a household and a valid profile with ordered, non-overlapping routines.", this);
            enabled = false;
        }
    }
}
