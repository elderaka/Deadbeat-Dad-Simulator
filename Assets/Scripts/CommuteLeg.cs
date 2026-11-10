using UnityEngine;

namespace BapakSimulator
{
    public sealed class CommuteLeg : MonoBehaviour
    {
        public enum Progress { Pending, Underway, Complete }

        [SerializeField] private PickupDay day;
        [SerializeField] private EscortFollower escort;
        [SerializeField] private bool returnTrip;
        [SerializeField] private string label;

        [SerializeField] private string persistentId;
        private int completedDay;

        internal string PersistentId => persistentId;
        internal bool WasCompletedOn(int dayNumber) => completedDay == dayNumber;
        internal void RestoreCompletedDay(int dayNumber) => completedDay = dayNumber;

        public string Label => label;
        public Progress State => day.DayNumber > 0 && completedDay == day.DayNumber ? Progress.Complete :
            escort.IsFollowing && escort.IsReturningHome == returnTrip ? Progress.Underway : Progress.Pending;

        private void Awake()
        {
            if (day != null && escort != null && !string.IsNullOrWhiteSpace(label)) return;

            Debug.LogError($"{nameof(CommuteLeg)} on '{name}' has incomplete route tracking.", this);
            enabled = false;
        }

        public void Complete()
        {
            if (isActiveAndEnabled && day.IsDayRunning)
                completedDay = day.DayNumber;
        }
    }
}
