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
        [SerializeField] private RectTransform touchPauseButton;
        private int lastAdvanceFrame = -1;

        private void OnEnable() => advance?.Enable();
        private void OnDisable() => advance?.Disable();

        private void Update()
        {
            var touch = Touchscreen.current?.primaryTouch;
            if (touch != null && touch.press.wasPressedThisFrame)
            {
                if (!HitsPause(touch.position.ReadValue())) Advance();
                return;
            }

            if (advance?.WasPressedThisFrame() != true) return;
            if (advance.activeControl?.device is Mouse mouse && HitsPause(mouse.position.ReadValue())) return;
            Advance();
        }

        private bool HitsPause(Vector2 position) =>
            touchPauseButton != null && touchPauseButton.gameObject.activeInHierarchy &&
            RectTransformUtility.RectangleContainsScreenPoint(touchPauseButton, position);

        public void Advance()
        {
            if (player.IsPaused || !runner.IsDialogueRunning || line.canvasGroup.alpha <= 0 ||
                options.interactable || options.blocksRaycasts || lastAdvanceFrame == Time.frameCount) return;

            lastAdvanceFrame = Time.frameCount;
            advancer.OnInputHurryUpLines();
        }
    }
}
