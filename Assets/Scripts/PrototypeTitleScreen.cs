using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BapakSimulator
{
    public sealed class PrototypeTitleScreen : MonoBehaviour
    {
        [SerializeField] private string gameplayScene;
        [SerializeField] private string gameplaySlot;
        [SerializeField] private DifficultyPreset[] presets = Array.Empty<DifficultyPreset>();
        [SerializeField] private DifficultyPreset defaultPreset;
        [SerializeField] private UnityEngine.UI.Slider difficulty;
        [SerializeField] private UnityEngine.UI.Toggle skipHistory;
        [SerializeField] private UnityEngine.UI.Text scaleLabels;
        [SerializeField] private UnityEngine.UI.Text scenarioTitle;
        [SerializeField] private UnityEngine.UI.Text scenarioDetails;
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject newGameMenu;
        [SerializeField] private UnityEngine.UI.Button startButton;
        [SerializeField] private UnityEngine.UI.Button continueButton;

        private bool loading;

        private DifficultyPreset Selected => presets[Mathf.RoundToInt(difficulty.value)];

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (!Guid.TryParseExact(gameplaySlot, "N", out _) || !Application.CanStreamedLevelBeLoaded(gameplayScene) ||
                presets.Length == 0 || presets.Any(p => p == null || !p.IsValid) ||
                presets.Select(p => p.Id).Distinct().Count() != presets.Length || !presets.Contains(defaultPreset) ||
                difficulty == null || skipHistory == null || scaleLabels == null || scenarioTitle == null ||
                scenarioDetails == null || mainMenu == null || newGameMenu == null || startButton == null || continueButton == null)
            {
                Debug.LogError("Title screen needs a gameplay scene and slot, unique presets, and UI references.", this);
                enabled = false;
                return;
            }

            difficulty.minValue = 0;
            difficulty.maxValue = presets.Length - 1;
            difficulty.wholeNumbers = true;
            difficulty.SetValueWithoutNotify(Array.IndexOf(presets, defaultPreset));
            scaleLabels.text = string.Join("     •     ", presets.Select(preset => preset.DisplayName));
            difficulty.onValueChanged.AddListener(_ => Refresh());
            skipHistory.onValueChanged.AddListener(_ => Refresh());
            ShowMainMenu();
            Refresh();
        }

        private void Refresh()
        {
            var preset = Selected;
            scenarioTitle.text = preset.DisplayName;
            string history = skipHistory.isOn || preset.InheritedDebt == 0
                ? "Utang lama: Rp0"
                : $"Utang lama: Rp{preset.InheritedDebt:N0} • Jatuh tempo hari {preset.InheritedDebtDueDay}";
            scenarioDetails.text = $"Tabungan awal: Rp{preset.StartingCash:N0}\n{history}";
        }

        public void ShowMainMenu()
        {
            if (loading) return;
            newGameMenu.SetActive(false);
            mainMenu.SetActive(true);
            continueButton.interactable = NewGameLaunch.FindContinueSlot(gameplaySlot) != null;
        }

        public void ShowNewGame()
        {
            if (loading) return;
            mainMenu.SetActive(false);
            newGameMenu.SetActive(true);
        }

        public void Quit() => Application.Quit();

        public void StartNewGame()
        {
            if (!isActiveAndEnabled || loading) return;
            Launch(Guid.NewGuid().ToString("N"), Selected, !skipHistory.isOn);
        }

        public void ContinueGame()
        {
            if (!isActiveAndEnabled || loading) return;
            string slot = NewGameLaunch.FindContinueSlot(gameplaySlot);
            if (slot != null) Launch(slot, null, false);
        }

        private void Launch(string slot, DifficultyPreset preset, bool includeHistory)
        {
            loading = true;
            NewGameLaunch.Set(new NewGameLaunch.Request
            {
                SceneSlot = gameplaySlot,
                RunSlot = slot,
                Preset = preset,
                IncludeHistory = includeHistory
            });
            SceneManager.LoadScene(gameplayScene);
        }
    }
}
