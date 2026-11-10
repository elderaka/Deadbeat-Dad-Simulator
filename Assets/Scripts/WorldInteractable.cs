using UnityEngine;
using UnityEngine.Events;

namespace BapakSimulator
{
    [DisallowMultipleComponent]
    public class WorldInteractable : MonoBehaviour
    {
        [SerializeField] private string prompt;
        [SerializeField] private UnityEvent onInteract = new UnityEvent();

        public virtual string Prompt => prompt;
        public UnityEvent OnInteract => onInteract;

        public virtual bool TryInteract()
        {
            if (!isActiveAndEnabled) return false;

            onInteract.Invoke();
            return true;
        }
    }
}
