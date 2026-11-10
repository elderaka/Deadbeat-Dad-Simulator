using UnityEngine;

namespace BapakSimulator
{
    public sealed class RiskPhoneApp : MonoBehaviour
    {
        [SerializeField] private RiskArcade arcade;
        [SerializeField] private PlayerController player;
        [SerializeField] private UnityEngine.UI.Text title;
        [SerializeField] private UnityEngine.UI.Text banner;
        [SerializeField] private UnityEngine.UI.Text status;
        [SerializeField] private UnityEngine.UI.Text ledger;
        [SerializeField] private UnityEngine.UI.Text playLabel;
        [SerializeField] private UnityEngine.UI.Text alternateLabel;
        [SerializeField] private UnityEngine.UI.Button play;
        [SerializeField] private UnityEngine.UI.Button alternate;
        [SerializeField] private UnityEngine.UI.Button next;
        [SerializeField, Min(0)] private float reelRevealSeconds;

        [SerializeField] private UnityEngine.UI.Text gameDisplay;
        [SerializeField] private UnityEngine.UI.Text[] reelSymbols;

        private RiskGameProfile displayedSlots;
        private string displayedOutcome;
        private int gameIndex;
        private int option;
        private float revealUntil;
        private RiskGameProfile Current => arcade.Profiles[gameIndex];

        private void Update()
        {
            if (!arcade.isActiveAndEnabled || arcade.Profiles.Count == 0) return;
            var profile = Current;
            bool delayed = profile.kind == RiskGameKind.Football || profile.kind == RiskGameKind.Togel;
            bool stock = profile.kind == RiskGameKind.Stock;
            bool crash = profile.kind == RiskGameKind.Crash;
            bool revealing = Time.time < revealUntil;
            title.text = $"{profile.displayName}  {gameIndex + 1}/{arcade.Profiles.Count}";
            banner.text = profile.banner;
            status.text = revealing ? "Gulungan berputar..." : arcade.GetStatus(profile);
            UpdateGameDisplay(profile, revealing);
            ledger.text = $"Keluar Rp{arcade.TotalSpent:N0}  •  Masuk Rp{arcade.TotalReturned:N0}";
            playLabel.text = stock ? "Beli 1 lot" : delayed ? "Pasang tiket" : crash ? "Mulai" : "PUTAR";
            play.interactable = !player.IsPaused && !revealing && arcade.CanPlay(profile);
            alternateLabel.text = stock ? "Jual 1 lot" : delayed ? "Ganti pilihan" : "Ambil hasil";
            bool alternateVisible = stock || delayed || crash;
            if (alternate.gameObject.activeSelf != alternateVisible) alternate.gameObject.SetActive(alternateVisible);
            alternate.interactable = !player.IsPaused && (stock ? arcade.CanSell(profile) : delayed || arcade.CanCashOut);
            next.interactable = !player.IsPaused && !arcade.IsCrashing && !revealing;
        }

        private void UpdateGameDisplay(RiskGameProfile profile, bool revealing)
        {
            if (gameDisplay == null) return;
            bool slots = profile.kind == RiskGameKind.Slots;
            gameDisplay.transform.parent.gameObject.SetActive(!slots);
            foreach (var symbol in reelSymbols) symbol.transform.parent.gameObject.SetActive(slots);

            if (!slots)
            {
                gameDisplay.text = profile.kind switch
                {
                    RiskGameKind.Crash => arcade.GetCrashDisplay(profile),
                    RiskGameKind.Stock => $"Rp{arcade.GetStockPrice(profile):N0}",
                    _ => profile.outcomes[option].label
                };
                return;
            }

            string outcome = revealing
                ? profile.outcomes[(int)(Time.time * 14) % profile.outcomes.Length].label
                : arcade.GetSlotResult(profile);
            if (profile == displayedSlots && outcome == displayedOutcome) return;

            var symbols = outcome?.Split('|');
            for (int i = 0; i < reelSymbols.Length; i++)
                reelSymbols[i].text = symbols != null && i < symbols.Length ? symbols[i].Trim() : "—";
            displayedSlots = profile;
            displayedOutcome = outcome;
        }

        private void OnDisable()
        {
            if (arcade != null) arcade.ForfeitCrash();
            revealUntil = 0;
        }

        public void NextGame()
        {
            if (player.IsPaused || arcade.IsCrashing || Time.time < revealUntil || arcade.Profiles.Count == 0) return;
            gameIndex = (gameIndex + 1) % arcade.Profiles.Count;
            option = 0;
        }

        public void Play()
        {
            if (player.IsPaused || Time.time < revealUntil || arcade.Profiles.Count == 0) return;
            if (arcade.Play(Current, option) && Current.kind == RiskGameKind.Slots)
                revealUntil = Time.time + reelRevealSeconds;
        }

        public void Alternate()
        {
            if (player.IsPaused || arcade.Profiles.Count == 0) return;
            switch (Current.kind)
            {
                case RiskGameKind.Stock: arcade.Sell(Current); break;
                case RiskGameKind.Crash: arcade.CashOut(); break;
                case RiskGameKind.Football:
                case RiskGameKind.Togel:
                    option = (option + 1) % Current.outcomes.Length;
                    break;
            }
        }
    }
}
