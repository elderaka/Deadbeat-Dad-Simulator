using System;
using System.Collections.Generic;
using UnityEngine;

namespace BapakSimulator
{
    [DisallowMultipleComponent]
    public sealed class NpcRoster : MonoBehaviour
    {
        [SerializeField] private NpcResident[] residents = Array.Empty<NpcResident>();
        private readonly Dictionary<string, NpcResident> byId = new();

        public IReadOnlyList<NpcResident> Residents => residents;

        public bool TryGetResident(RelationshipDefinition identity, out NpcResident resident)
        {
            resident = null;
            return identity != null && !string.IsNullOrWhiteSpace(identity.PersistentId) &&
                byId.TryGetValue(identity.PersistentId, out resident);
        }

        private void Awake()
        {
            foreach (var resident in residents)
            {
                if (resident != null && resident.IsConfigured &&
                    byId.TryAdd(resident.Profile.Identity.PersistentId, resident)) continue;

                Debug.LogError($"NPC roster '{name}' has a missing, invalid or duplicate resident: {resident}.", this);
                byId.Clear();
                enabled = false;
                return;
            }
        }
    }
}
