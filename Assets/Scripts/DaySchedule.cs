using UnityEngine;

namespace BapakSimulator
{
    [CreateAssetMenu(menuName = "Bapak Simulator/Day Schedule")]
    public sealed class DaySchedule : ScriptableObject
    {
        [SerializeField, Min(0)] private int startMinute;
        [SerializeField, Min(0)] private int pickupStartMinute;
        [SerializeField, Min(1)] private int pickupDeadline;
        [SerializeField, Min(1)] private int endMinute;
        [SerializeField, Min(1)] private int pickupDuration;
        [SerializeField, Min(1)] private int fishingDuration;
        [SerializeField, Min(0.1f)] private float minutesPerSecond;

        public int StartMinute => startMinute;
        public int PickupStartMinute => pickupStartMinute;
        public int PickupDeadline => pickupDeadline;
        public int EndMinute => endMinute;
        public int PickupDuration => pickupDuration;
        public int FishingDuration => fishingDuration;
        public float MinutesPerSecond => minutesPerSecond;

        public string ValidationError
        {
            get
            {
                if (startMinute < 0 || startMinute >= endMinute || endMinute > 24 * 60)
                    return "Start Minute and End Minute must define a positive day within 00:00–24:00.";
                if (pickupStartMinute < startMinute || pickupStartMinute >= pickupDeadline || pickupDeadline > endMinute)
                    return "Pickup Start Minute and Pickup Deadline must define a window within the day.";
                if (pickupDuration <= 0 || pickupDuration > pickupDeadline - pickupStartMinute)
                    return "Pickup Duration must fit inside the pickup window.";
                if (fishingDuration <= 0 || fishingDuration > endMinute - startMinute)
                    return "Fishing Duration must fit inside the day.";
                if (minutesPerSecond <= 0 || float.IsNaN(minutesPerSecond) || float.IsInfinity(minutesPerSecond))
                    return "Minutes Per Second must be finite and positive.";
                return null;
            }
        }
    }
}
