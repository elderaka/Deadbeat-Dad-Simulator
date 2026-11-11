using UnityEngine;

namespace BapakSimulator
{
    public sealed class TouchControls : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private PhonePanel phone;
        [SerializeField] private GameObject walkingControls;
        [SerializeField] private GameObject pauseControl;
        [SerializeField] private GameObject phoneCloseControl;
        [SerializeField] private TouchDragSurface movement;
        [SerializeField] private TouchDragSurface cameraLook;
        [SerializeField] private UnityEngine.UI.Text modeLabel;
        [SerializeField, Range(.02f, .3f)] private float movementRadius = .12f;

        public bool ModeEnabled { get; private set; }
        private bool jumpQueued;

        public void ToggleMode()
        {
            ModeEnabled = !ModeEnabled;
            modeLabel.text = ModeEnabled ? "Kontrol sentuh: Aktif" : "Kontrol sentuh: Mati";
            RefreshVisibility();
        }

        public void Jump()
        {
            if (walkingControls.activeInHierarchy && player.CanOpenDialogue) jumpQueued = true;
        }

        public void Interact()
        {
            if (walkingControls.activeInHierarchy && player.CanOpenDialogue) player.TryInteract();
        }

        internal void Read(out Vector2 move, out Vector2 look, out bool jump)
        {
            move = Vector2.ClampMagnitude(movement.Displacement / Mathf.Max(1, Screen.height * movementRadius), 1);
            look = cameraLook.ConsumeDelta();
            jump = jumpQueued;
            jumpQueued = false;
        }

        private void LateUpdate() => RefreshVisibility();

        private void RefreshVisibility()
        {
            bool available = ModeEnabled && !player.IsPaused && player.ActiveVehicle == null;
            bool walking = available && player.CanOpenDialogue && Application.isFocused;
            walkingControls.SetActive(walking);
            pauseControl.SetActive(available);
            phoneCloseControl.SetActive(available && phone.IsOpen);
            if (!walking) jumpQueued = false;
        }
    }
}
