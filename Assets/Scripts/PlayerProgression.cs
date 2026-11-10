using UnityEngine;

namespace BapakSimulator
{
    [DisallowMultipleComponent]
    public sealed class PlayerProgression : MonoBehaviour
    {
        [SerializeField, Min(0)] private int startingCash;

        private readonly System.Collections.Generic.Dictionary<string, int> relationships = new();

        internal System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, int>>
            Relationships => relationships;

        public int GetRelationship(RelationshipDefinition relationship) =>
            relationship != null && !string.IsNullOrWhiteSpace(relationship.PersistentId) &&
            relationships.TryGetValue(relationship.PersistentId, out int value) ? value : 0;

        public bool TrySetRelationship(RelationshipDefinition relationship, int value)
        {
            if (relationship == null || string.IsNullOrWhiteSpace(relationship.PersistentId)) return false;
            relationships[relationship.PersistentId] = value;
            return true;
        }

        private readonly System.Collections.Generic.HashSet<string> npcFlags = new();
        internal System.Collections.Generic.IEnumerable<string> NpcFlags => npcFlags;

        internal bool HasNpcFlag(RelationshipDefinition identity, string flag) =>
            npcFlags.Contains(identity.PersistentId + "/" + flag);

        internal void SetNpcFlag(RelationshipDefinition identity, string flag)
        {
            if (!string.IsNullOrWhiteSpace(flag)) npcFlags.Add(identity.PersistentId + "/" + flag);
        }

        internal void RestoreCheckpoint(CheckpointData data)
        {
            Cash = data.cash;
            relationships.Clear();
            npcFlags.Clear();
            npcFlags.UnionWith(data.npcFlags);
            foreach (var entry in data.relationships) relationships.Add(entry.id, entry.value);
        }

        public int Cash { get; private set; }
        internal bool HasValidStartingCash => startingCash >= 0;

        private void Awake()
        {
            if (HasValidStartingCash)
                Cash = startingCash;
            else
                Debug.LogError($"{nameof(PlayerProgression)} on '{name}' needs nonnegative Starting Cash.", this);
        }

        internal void SetStartingCash(int amount) => Cash = amount;

        internal bool TrySpendCash(int amount)
        {
            if (amount <= 0 || amount > Cash) return false;
            Cash -= amount;
            return true;
        }

        internal void CreditCash(int amount)
        {
            if (amount > 0) Cash = (int)System.Math.Min(int.MaxValue, (long)Cash + amount);
        }
    }
}
