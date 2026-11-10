using UnityEngine;
using Yarn.Unity;

namespace BapakSimulator
{
    [DisallowMultipleComponent, RequireComponent(typeof(NpcResident))]
    public sealed class NeighborBuyout : MonoBehaviour
    {
        [SerializeField] private PickupDay household;
        [SerializeField] private HouseholdFinances finances;
        [SerializeField] private ItemDefinition householdItem;
        [SerializeField] private GameObject itemVisual;
        [SerializeField, Min(1)] private int referenceValue;
        [SerializeField, Min(1)] private int offeredPrice;
        [SerializeField] private string knowledgeFlag;
        [SerializeField] private string declinedFlag;
        [SerializeField] private string soldFlag;
        [SerializeField] private Transform hangout;

        private NpcResident resident;

        internal Transform Hangout => hangout;
        private RelationshipDefinition Identity => resident.Profile.Identity;
        private PlayerProgression Progression => household.Progression;
        private bool Knows => Progression.HasNpcFlag(Identity, knowledgeFlag);
        private bool Declined => Progression.HasNpcFlag(Identity, declinedFlag);
        private bool Sold => Progression.HasNpcFlag(Identity, soldFlag);
        private bool Eligible => isActiveAndEnabled && household.IsConfigured && household.IsDayRunning &&
            (finances.BillOverdue || finances.LoanOverdue);

        private void Awake()
        {
            resident = GetComponent<NpcResident>();
            if (resident.IsConfigured && household != null && finances != null && householdItem != null &&
                householdItem.IsValid && itemVisual != null && offeredPrice > 0 && offeredPrice < referenceValue &&
                !string.IsNullOrWhiteSpace(knowledgeFlag) && !string.IsNullOrWhiteSpace(declinedFlag) &&
                !string.IsNullOrWhiteSpace(soldFlag) && knowledgeFlag != declinedFlag &&
                knowledgeFlag != soldFlag && declinedFlag != soldFlag) return;
            Debug.LogError($"Neighbor buyout on '{name}' needs an item, low offer and distinct memory flags.", this);
            enabled = false;
        }

        private void LateUpdate()
        {
            if (household.IsConfigured && itemVisual.activeSelf == Sold) itemVisual.SetActive(!Sold);
        }

        internal void Prepare(VariableStorageBehaviour variables)
        {
            bool configured = isActiveAndEnabled && household.IsConfigured;
            variables.SetValue("$buyout_disclose", configured && Eligible && !Knows && !Declined && !Sold);
            variables.SetValue("$buyout_available", configured && Eligible && Knows && !Declined && !Sold);
            variables.SetValue("$buyout_declined", configured && Declined);
            variables.SetValue("$buyout_sold", configured && Sold);
            if (!configured) return;
            variables.SetValue("$buyout_item", householdItem.DisplayName);
            variables.SetValue("$buyout_price", offeredPrice.ToString("N0"));
            variables.SetValue("$buyout_value", referenceValue.ToString("N0"));
        }

        internal bool Execute(string action)
        {
            if (!Eligible || Declined || Sold) return false;
            if (action == "disclose_finances")
            {
                Progression.SetNpcFlag(Identity, knowledgeFlag);
                return true;
            }

            if (!Knows) return false;
            switch (action)
            {
                case "decline_buyout":
                    Progression.SetNpcFlag(Identity, declinedFlag);
                    return true;
                case "accept_buyout":
                    if (offeredPrice > int.MaxValue - household.Cash) return false;
                    Progression.SetNpcFlag(Identity, soldFlag);
                    Progression.CreditCash(offeredPrice);
                    itemVisual.SetActive(false);
                    household.ReportActivity($"{householdItem.DisplayName} terjual • +Rp{offeredPrice:N0}.");
                    return true;
                default:
                    return false;
            }
        }
    }
}
