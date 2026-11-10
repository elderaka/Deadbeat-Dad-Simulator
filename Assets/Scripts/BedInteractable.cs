using UnityEngine;

namespace BapakSimulator
{
    public sealed class BedInteractable : WorldInteractable
    {
        [SerializeField] private PickupDay household;
        [SerializeField] private string sleepPrompt;
        [SerializeField] private string morningPrompt;

        public override string Prompt => household != null && household.IsDayRunning ? sleepPrompt : morningPrompt;

        public override bool TryInteract()
        {
            if (!isActiveAndEnabled || household == null || !household.IsConfigured) return false;
            household.UseHome();
            return true;
        }
    }
}
