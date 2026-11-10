using UnityEngine;

namespace BapakSimulator
{
    [DisallowMultipleComponent]
    public sealed class SchoolPickupDialogue : MonoBehaviour
    {
        [SerializeField] private PickupDay household;
        [SerializeField] private NpcResident caller;
        [SerializeField] private ResidentEscortDialogue escortDialogue;
        [SerializeField, Range(0, 1439)] private int ringMinute;

        private int answeredDay;

        public string CallerName => caller.Profile.DisplayName;
        public bool HasIncomingCall => isActiveAndEnabled && household.CanPromiseManualPickup && !household.ManualPickupPromised &&
            escortDialogue.IsAtReturnStop && household.Minute >= ringMinute && answeredDay != household.DayNumber;

        private void Awake()
        {
            if (household != null && caller != null && caller.IsConfigured && escortDialogue != null) return;
            Debug.LogError($"School pickup dialogue '{name}' has incomplete references.", this);
            enabled = false;
        }

        public void Call()
        {
            if (!isActiveAndEnabled) return;
            if (!TryAnswerCall()) escortDialogue.TryTalk(true);
        }

        internal void DismissIncomingCall()
        {
            if (HasIncomingCall) answeredDay = household.DayNumber;
        }

        public bool TryAnswerCall()
        {
            if (!HasIncomingCall || !escortDialogue.TryTalk(true)) return false;
            answeredDay = household.DayNumber;
            return true;
        }
    }
}
