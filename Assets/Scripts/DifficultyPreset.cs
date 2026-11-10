using UnityEngine;

namespace BapakSimulator
{
    [CreateAssetMenu(menuName = "Bapak Simulator/New Game/Difficulty Preset")]
    public sealed class DifficultyPreset : ScriptableObject
    {
        [SerializeField] private string persistentId;
        [SerializeField] private string displayName;
        [SerializeField] private string scenarioTitle;
        [SerializeField, TextArea] private string description;
        [SerializeField] private string flavorLine;
        [SerializeField, Min(0)] private int startingCash;
        [SerializeField, Min(0)] private int inheritedDebt;
        [SerializeField, Min(1)] private int inheritedDebtDueDay;

        public string Id => persistentId;
        public string DisplayName => displayName;
        public string ScenarioTitle => scenarioTitle;
        public string Description => description;
        public string FlavorLine => flavorLine;
        public int StartingCash => startingCash;
        public int InheritedDebt => inheritedDebt;
        public int InheritedDebtDueDay => inheritedDebtDueDay;
        internal bool IsValid => !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(DisplayName) &&
            !string.IsNullOrWhiteSpace(ScenarioTitle) && startingCash >= 0 && inheritedDebt >= 0 && inheritedDebtDueDay > 0;
    }
}
