using System.Linq;
using UnityEngine;

namespace BapakSimulator
{
    [DefaultExecutionOrder(-20), DisallowMultipleComponent, RequireComponent(typeof(EscortFollower), typeof(NpcResident))]
    public sealed class FactoryPickupRequest : MonoBehaviour
    {
        public enum RequestState { None, Waiting, Promised, Declined, Expired, Completed }

        [SerializeField] private PickupDay household;
        [SerializeField] private NpcRoutineLocation shiftLocation;
        [SerializeField, Min(1)] private int waitMinutes;
        [SerializeField, Min(1)] private int ringMinutes;

        private EscortFollower escort;
        private NpcResident resident;
        private ResidentEscortDialogue dialogue;
        private int silencedDay;
        private int requestDay;
        private int requestMinute;
        private RequestState state;

        public string CallerName => resident.Profile.DisplayName;
        public int Deadline => requestMinute + waitMinutes;
        public RequestState State => household != null && requestDay == household.DayNumber ? state : RequestState.None;
        public bool IsRinging => isActiveAndEnabled && State == RequestState.Waiting && household.IsDayRunning &&
            silencedDay != household.DayNumber &&
            household.Minute < requestMinute + ringMinutes;

        private void Awake()
        {
            escort = GetComponent<EscortFollower>();
            resident = GetComponent<NpcResident>();
            dialogue = GetComponent<ResidentEscortDialogue>();
            var shift = resident.Profile?.Routine.FirstOrDefault(entry => entry.location == shiftLocation) ?? default;
            requestMinute = shift.endMinute;
            if (household != null && shiftLocation != null && requestMinute > 0 && waitMinutes > 0 && ringMinutes > 0 && ringMinutes <= waitMinutes && Deadline <= 1440)
                return;

            Debug.LogError($"Factory pickup request on '{name}' needs a household, authored shift location, and valid waiting window.", this);
            enabled = false;
        }

        private void OnEnable() => escort.ArrivedHome.AddListener(Complete);

        private void OnDisable()
        {
            if (escort != null) escort.ArrivedHome.RemoveListener(Complete);
            if (resident != null) resident.HoldRoutineTravel = false;
        }

        private void Update()
        {
            if (!household.IsDayRunning)
            {
                resident.HoldRoutineTravel = false;
                return;
            }

            if (requestDay != household.DayNumber)
            {
                requestDay = household.DayNumber;
                state = RequestState.None;
            }

            if (state == RequestState.None && household.Minute >= requestMinute && household.Minute < Deadline &&
                escort.IsReturningHome && !escort.IsFollowing)
                state = RequestState.Waiting;

            if ((state == RequestState.Waiting || state == RequestState.Promised) &&
                household.Minute >= Deadline && !escort.IsFollowing)
                state = RequestState.Expired;

            resident.HoldRoutineTravel = state == RequestState.Waiting || state == RequestState.Promised;
        }

        internal void SilenceIncomingCall()
        {
            if (IsRinging) silencedDay = household.DayNumber;
        }

        internal bool TryAnswerCall()
        {
            if (!IsRinging || dialogue == null || !dialogue.TryTalk(true)) return false;
            SilenceIncomingCall();
            return true;
        }

        internal bool TryRespond(bool accept)
        {
            if (State != RequestState.Waiting || !household.IsDayRunning || household.Minute >= Deadline) return false;
            state = accept ? RequestState.Promised : RequestState.Declined;
            if (!accept) resident.HoldRoutineTravel = false;
            return true;
        }

        private void Complete()
        {
            if (State != RequestState.Promised && State != RequestState.Waiting) return;
            state = RequestState.Completed;
            resident.HoldRoutineTravel = false;
        }
    }
}
