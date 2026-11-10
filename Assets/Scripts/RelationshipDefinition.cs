using UnityEngine;

namespace BapakSimulator
{
    [CreateAssetMenu(menuName = "Bapak Simulator/Progression/Relationship")]
    public sealed class RelationshipDefinition : ScriptableObject
    {
        [SerializeField] private string persistentId;
        internal string PersistentId => persistentId;
    }
}
