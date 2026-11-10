using UnityEngine;

namespace BapakSimulator
{
    [CreateAssetMenu(menuName = "Bapak Simulator/Economy/Buy Offer")]
    public sealed class BuyOffer : ScriptableObject
    {
        [SerializeField] private ItemDefinition item;
        [SerializeField, Min(1)] private int unitPrice;

        public ItemDefinition Item => item;
        public int UnitPrice => unitPrice;
        public bool IsValid => item != null && item.IsValid && unitPrice > 0;
    }
}
