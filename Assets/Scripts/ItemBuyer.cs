using UnityEngine;

namespace BapakSimulator
{
    public sealed class ItemBuyer : WorldInteractable
    {
        [SerializeField] private PickupDay household;
        [SerializeField] private ItemInventory inventory;
        [SerializeField] private NeighborhoodDialogue dialogue;
        [SerializeField] private string dialogueNode;
        [SerializeField] private BuyOffer offer;

        private void Awake()
        {
            if (household != null && inventory != null && dialogue != null && !string.IsNullOrWhiteSpace(dialogueNode) && offer != null && offer.IsValid)
                return;

            Debug.LogError($"{nameof(ItemBuyer)} on '{name}' needs a wallet, bag, dialogue, and valid buy offer.", this);
            enabled = false;
        }

        public override bool TryInteract()
        {
            if (!isActiveAndEnabled || !household.IsConfigured) return false;

            var item = offer.Item;
            int count = inventory.Count(item);
            int price = offer.UnitPrice;
            return dialogue.TryStart(dialogueNode, variables =>
            {
                variables.SetValue("$sale_item", item.DisplayName);
                variables.SetValue("$sale_count", count);
                variables.SetValue("$sale_price", price.ToString("N0"));
                variables.SetValue("$sale_total", ((long)count * price).ToString("N0"));
            }, action => action switch
            {
                "sell_one" => Sell(item, 1, price),
                "sell_all" => Sell(item, count, price),
                _ => false
            });
        }

        private bool Sell(ItemDefinition item, int count, int unitPrice)
        {
            long total = (long)count * unitPrice;
            if (!isActiveAndEnabled || !household.IsConfigured || total > int.MaxValue - household.Cash ||
                !inventory.TryRemove(item, count))
            {
                household.ReportActivity("Penjualan gagal. Periksa isi tas dan saldo.");
                return false;
            }

            household.Progression.CreditCash((int)total);
            household.ReportActivity($"Terjual: {item.DisplayName} ×{count} • +Rp{total}.");
            return true;
        }
    }
}
