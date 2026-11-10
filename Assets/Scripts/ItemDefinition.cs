using UnityEngine;

namespace BapakSimulator
{
    [CreateAssetMenu(menuName = "Bapak Simulator/Economy/Item")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string displayName;

        [SerializeField] private string persistentId;
        internal string PersistentId => persistentId;

        public string DisplayName => displayName;
        public bool IsValid => !string.IsNullOrWhiteSpace(displayName);
    }
}
