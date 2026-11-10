using UnityEngine;

namespace BapakSimulator
{
    public sealed class TransportDialogue : MonoBehaviour
    {
        [SerializeField] private NeighborhoodDialogue dialogue;
        [SerializeField] private PickupDay household;
        [SerializeField] private string node;

        public void Talk()
        {
            if (!isActiveAndEnabled || dialogue == null || household == null || !household.IsConfigured) return;
            int dayNumber = household.DayNumber;
            dialogue.TryStart(node, variables =>
            {
                variables.SetValue("$transport_fee", household.TransportFee.ToString("N0"));
                variables.SetValue("$transport_recurring", household.HasRecurringTransport);
                variables.SetValue("$pickup_open", household.CanPromiseManualPickup);
            }, action => action == "arrange_transport" && household.DayNumber == dayNumber &&
                household.TryArrangeTransport());
        }
    }
}
