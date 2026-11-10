using UnityEngine;

namespace BapakSimulator
{
    [DisallowMultipleComponent, RequireComponent(typeof(EscortFollower), typeof(NpcResident))]
    public sealed class ResidentEscortDialogue : MonoBehaviour
    {
        [SerializeField] private NeighborhoodDialogue dialogue;
        [SerializeField] private PickupDay household;
        [SerializeField] private string node;
        [SerializeField] private string pickupMentionedFlag;

        private EscortFollower escort;
        private NpcResident resident;
        private FactoryPickupRequest factoryPickup;

        private void Awake()
        {
            escort = GetComponent<EscortFollower>();
            resident = GetComponent<NpcResident>();
            factoryPickup = GetComponent<FactoryPickupRequest>();
        }

        internal bool IsAtReturnStop => escort.IsReturningHome && !escort.IsFollowing;

        public void Call()
        {
            if (TryTalk(true) && factoryPickup != null) factoryPickup.SilenceIncomingCall();
        }

        internal bool TryTalk(bool byPhone = false)
        {
            if (!isActiveAndEnabled || dialogue == null || household == null || !household.IsConfigured ||
                !resident.IsConfigured || resident.IsTalkingInPerson) return false;

            int dayNumber = household.DayNumber;
            var identity = resident.Profile.Identity;
            var progression = household.Progression;
            resident.IsTalkingInPerson = !byPhone;
            bool started = dialogue.TryStart(node, variables =>
            {
                variables.SetValue("$by_phone", byPhone);
                variables.SetValue("$factory_waiting", factoryPickup != null &&
                    factoryPickup.State == FactoryPickupRequest.RequestState.Waiting);
                variables.SetValue("$factory_promised", factoryPickup != null &&
                    factoryPickup.State == FactoryPickupRequest.RequestState.Promised);
                variables.SetValue("$factory_deadline", factoryPickup == null ? "" :
                    $"{factoryPickup.Deadline / 60:00}:{factoryPickup.Deadline % 60:00}");
                variables.SetValue("$return_trip", escort.IsReturningHome);
                variables.SetValue("$pickup_open", household.CanPromiseManualPickup);
                variables.SetValue("$pickup_promised", household.ManualPickupPromised);
                variables.SetValue("$pickup_mentioned", !string.IsNullOrEmpty(pickupMentionedFlag) &&
                    progression.HasNpcFlag(identity, pickupMentionedFlag));
                variables.SetValue("$affinity", (float)progression.GetRelationship(identity));
            }, action =>
            {
                if (household.DayNumber != dayNumber) return false;
                switch (action)
                {
                    case "accept_factory_pickup": return factoryPickup != null && factoryPickup.TryRespond(true);
                    case "decline_factory_pickup": return factoryPickup != null && factoryPickup.TryRespond(false);
                    case "escort": return !byPhone && escort.TryStartFollowing();
                    case "ask_coverage": return household.TryAskIbu();
                    case "promise_pickup": return household.TryPromiseManualPickup();
                    case "remember_pickup":
                        progression.SetNpcFlag(identity, pickupMentionedFlag);
                        return true;
                    default: return false;
                }
            }, () => resident.IsTalkingInPerson = false);
            if (!started) resident.IsTalkingInPerson = false;
            return started;
        }
    }
}
