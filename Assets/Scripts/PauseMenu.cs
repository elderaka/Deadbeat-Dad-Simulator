using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BapakSimulator
{
    [DefaultExecutionOrder(-100), DisallowMultipleComponent, RequireComponent(typeof(PlayerController))]
    public sealed class PauseMenu : MonoBehaviour
    {
        [SerializeField] private InputAction togglePause;
        [SerializeField] private GameObject panel;
        [SerializeField] private UnityEngine.UI.Button resumeButton;
        [SerializeField] private EventSystem eventSystem;

        private PlayerController player;
        private float previousTimeScale;
        private CursorLockMode previousCursorLock;
        private bool previousCursorVisibility;
        private bool previousTouchMode;
        private GameObject previousSelection;

        private void Awake()
        {
            player = GetComponent<PlayerController>();
            if (togglePause != null && togglePause.bindings.Count > 0 && panel != null &&
                resumeButton != null && eventSystem != null)
            {
                panel.SetActive(false);
                return;
            }

            Debug.LogError($"Pause Menu on '{name}' needs an input binding, panel, resume button, and EventSystem.", this);
            enabled = false;
        }

        private void OnEnable()
        {
            togglePause.Enable();
            resumeButton.onClick.AddListener(Resume);
        }

        private void OnDisable()
        {
            togglePause?.Disable();
            if (resumeButton != null) resumeButton.onClick.RemoveListener(Resume);
            Resume();
        }

        private void Update()
        {
            if (!togglePause.WasPressedThisFrame()) return;
            if (player.IsPaused) Resume();
            else Open();
        }

        public void Open()
        {
            if (!isActiveAndEnabled || player.IsPaused) return;
            previousTouchMode = player.UsesTouchControls;
            previousTimeScale = Time.timeScale;
            previousCursorLock = Cursor.lockState;
            previousCursorVisibility = Cursor.visible;
            previousSelection = eventSystem.currentSelectedGameObject;
            player.SetPaused(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            panel.SetActive(true);
            eventSystem.SetSelectedGameObject(resumeButton.gameObject);
        }

        private void Resume()
        {
            if (player == null || !player.IsPaused) return;
            Time.timeScale = previousTimeScale;
            Cursor.lockState = previousCursorLock;
            Cursor.visible = previousCursorVisibility;
            if (panel != null) panel.SetActive(false);
            if (eventSystem != null)
                eventSystem.SetSelectedGameObject(previousSelection != null && previousSelection.activeInHierarchy
                    ? previousSelection : null);
            player.SetPaused(false);
            if (player.UsesTouchControls && player.CanOpenDialogue)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (previousTouchMode && player.CanOpenDialogue && !Application.isMobilePlatform)
                Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
