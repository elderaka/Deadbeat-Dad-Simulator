using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

namespace BapakSimulator
{
    public sealed class DialogueAdvanceInput : MonoBehaviour
    {
        [SerializeField] private DialogueRunner runner;
        [SerializeField] private LineAdvancer advancer;
        [SerializeField] private LinePresenter line;
        [SerializeField] private CanvasGroup options;
        [SerializeField] private PlayerController player;
        [SerializeField] private InputAction advance;
        private int lastAdvanceFrame = -1;

        private void OnEnable() => advance?.Enable();
        private void OnDisable() => advance?.Disable();

        private void Update()
        {
            if (advance?.WasPressedThisFrame() == true) Advance();
        }

        public void Advance()
        {
            if (player.IsPaused || !runner.IsDialogueRunning || line.canvasGroup.alpha <= 0 ||
                options.interactable || options.blocksRaycasts || lastAdvanceFrame == Time.frameCount) return;
            lastAdvanceFrame = Time.frameCount;
            advancer.RequestNextLine();
        }
    }
}
