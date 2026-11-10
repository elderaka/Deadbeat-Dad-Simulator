using System.Collections.Generic;
using UnityEngine;

namespace BapakSimulator
{
    public sealed class ItemInventory : MonoBehaviour
    {
        private readonly Dictionary<ItemDefinition, int> quantities = new Dictionary<ItemDefinition, int>();

        internal IEnumerable<KeyValuePair<ItemDefinition, int>> Stacks => quantities;

        public int TotalCount { get; private set; }
        public int Count(ItemDefinition item) => item != null && quantities.TryGetValue(item, out int count)
            ? count : 0;

        public bool TryAdd(ItemDefinition item, int quantity)
        {
            if (item == null || !item.IsValid || quantity <= 0 || quantity > int.MaxValue - TotalCount)
                return false;

            quantities[item] = Count(item) + quantity;
            TotalCount += quantity;
            return true;
        }

        public bool TryRemove(ItemDefinition item, int quantity)
        {
            int count = Count(item);
            if (quantity <= 0 || quantity > count) return false;

            int remaining = count - quantity;
            if (remaining == 0) quantities.Remove(item);
            else quantities[item] = remaining;
            TotalCount -= quantity;
            return true;
        }
    }
}
