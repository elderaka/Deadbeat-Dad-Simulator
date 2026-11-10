using System;
using System.Collections.Generic;
using UnityEngine;

namespace BapakSimulator
{
    [DisallowMultipleComponent]
    public sealed class DominoTable : WorldInteractable
    {
        private const int HandSize = 5;
        private const int SeatCount = 4;

        [SerializeField] private PickupDay household;
        [SerializeField] private PlayerController player;
        [SerializeField] private NpcProfile[] neighbors = Array.Empty<NpcProfile>();
        [SerializeField, Min(1)] private int stake;
        [SerializeField, Min(1)] private int minutesPerTurn;
        [SerializeField] private GameObject panel;
        [SerializeField] private UnityEngine.UI.Text board;
        [SerializeField] private UnityEngine.UI.Text status;
        [SerializeField] private UnityEngine.UI.Button casual;
        [SerializeField] private UnityEngine.UI.Button wager;
        [SerializeField] private UnityEngine.UI.Button pass;
        [SerializeField] private UnityEngine.UI.Button close;
        [SerializeField] private UnityEngine.UI.Button[] tileButtons = Array.Empty<UnityEngine.UI.Button>();

        private readonly List<Vector2Int>[] hands = new List<Vector2Int>[SeatCount];
        private readonly UnityEngine.UI.Text[] tileLabels = new UnityEngine.UI.Text[HandSize];
        private bool open;
        private bool playing;
        private bool hasBoard;
        private int left;
        private int right;
        private int passes;
        private int currentStake;
        private CursorLockMode previousLock;
        private bool previousVisibility;

        private void Awake()
        {
            if (household == null || player == null || panel == null || board == null || status == null ||
                casual == null || wager == null || pass == null || close == null ||
                neighbors.Length != SeatCount - 1 || Array.Exists(neighbors, profile => profile == null) ||
                tileButtons.Length != HandSize || Array.Exists(tileButtons, button => button == null ||
                    button.GetComponentInChildren<UnityEngine.UI.Text>(true) == null) ||
                stake <= 0 || stake > int.MaxValue / SeatCount || minutesPerTurn <= 0)
            {
                Debug.LogError($"Domino table '{name}' has incomplete seats, rules, or UI.", this);
                enabled = false;
                return;
            }

            for (int i = 0; i < SeatCount; i++) hands[i] = new List<Vector2Int>(HandSize);
            for (int i = 0; i < HandSize; i++)
            {
                int index = i;
                tileLabels[i] = tileButtons[i].GetComponentInChildren<UnityEngine.UI.Text>(true);
                tileButtons[i].onClick.AddListener(() => TakeTurn(index));
            }

            casual.onClick.AddListener(() => StartRound(0));
            wager.onClick.AddListener(() => StartRound(stake));
            pass.onClick.AddListener(() => TakeTurn(-1));
            close.onClick.AddListener(Close);
            panel.SetActive(false);
        }

        public override bool TryInteract()
        {
            if (!isActiveAndEnabled || open || !player.CanOpenDialogue || !household.IsDayRunning) return false;
            open = true;
            previousLock = Cursor.lockState;
            previousVisibility = Cursor.visible;
            player.SetGameplayInputBlocked(true);
            player.SetWorldInteractionBlocked(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            panel.SetActive(true);
            status.text = $"Santai, atau ikut taruhan Rp{stake:N0}. Keluar saat ronde berarti menyerah.";
            Refresh();
            return true;
        }

        private void OnDisable() => Close();

        private void Close()
        {
            if (!open) return;
            if (playing)
            {
                playing = false;
                household.ReportActivity("Bapak meninggalkan ronde domino.");
            }
            open = false;
            panel.SetActive(false);
            player.SetGameplayInputBlocked(false);
            player.SetWorldInteractionBlocked(false);
            Cursor.lockState = previousLock;
            Cursor.visible = previousVisibility;
        }

        private void StartRound(int amount)
        {
            if (!open || playing || player.IsPaused || !household.IsDayRunning ||
                (amount > 0 && !household.TrySpendCash(amount))) return;
            currentStake = amount;
            hasBoard = false;
            passes = 0;
            playing = true;
            var deck = new List<Vector2Int>();
            for (int a = 0; a <= 6; a++)
                for (int b = a; b <= 6; b++)
                    deck.Add(new Vector2Int(a, b));
            foreach (var hand in hands)
            {
                hand.Clear();
                for (int i = 0; i < HandSize; i++)
                {
                    int draw = UnityEngine.Random.Range(0, deck.Count);
                    hand.Add(deck[draw]);
                    deck.RemoveAt(draw);
                }
            }
            status.text = "Bapak mulai. Pilih ubin yang cocok dengan salah satu ujung.";
            Refresh();
        }

        private bool Fits(Vector2Int tile) => !hasBoard ||
            tile.x == left || tile.y == left || tile.x == right || tile.y == right;

        private int BestTile(List<Vector2Int> hand)
        {
            int best = -1;
            for (int i = 0; i < hand.Count; i++)
                if (Fits(hand[i]) && (best < 0 || Pips(hand[i]) > Pips(hand[best]))) best = i;
            return best;
        }

        private static int Pips(Vector2Int tile) => tile.x + tile.y;

        private void TakeTurn(int index)
        {
            if (!open || !playing || player.IsPaused || !household.IsDayRunning) return;
            if (index < 0 ? BestTile(hands[0]) >= 0 : index >= hands[0].Count || !Fits(hands[0][index])) return;

            if (!household.TryUseLeisureTime(minutesPerTurn))
            {
                playing = false;
                status.text = "Waktu habis. Ronde berhenti.";
                Refresh();
                return;
            }

            Play(0, index);
            for (int seat = 1; seat < SeatCount && playing; seat++)
                Play(seat, BestTile(hands[seat]));
            if (playing) status.text = $"Giliran Bapak • {minutesPerTurn} menit per putaran.";
            Refresh();
        }

        private void Play(int seat, int index)
        {
            if (index < 0)
            {
                passes++;
                if (passes == SeatCount) Finish(BlockedWinner());
                return;
            }

            passes = 0;
            var tile = hands[seat][index];
            hands[seat].RemoveAt(index);
            if (!hasBoard)
            {
                hasBoard = true;
                left = tile.x;
                right = tile.y;
            }
            else if (tile.x == left || tile.y == left)
                left = tile.x == left ? tile.y : tile.x;
            else
                right = tile.x == right ? tile.y : tile.x;
            if (hands[seat].Count == 0) Finish(seat);
        }

        private int BlockedWinner()
        {
            int best = int.MaxValue;
            int winner = -1;
            for (int seat = 0; seat < SeatCount; seat++)
            {
                int total = 0;
                foreach (var tile in hands[seat]) total += Pips(tile);
                if (total < best) { best = total; winner = seat; }
                else if (total == best) winner = -1;
            }
            return winner;
        }

        private void Finish(int winner)
        {
            playing = false;
            int returned = winner == 0 ? currentStake * SeatCount : winner < 0 ? currentStake : 0;
            int paid = Mathf.Min(returned, int.MaxValue - household.Cash);
            if (paid > 0) household.Progression.CreditCash(paid);
            string result = winner < 0 ? "Seri." : winner == 0 ? "Bapak menang." : $"{neighbors[winner - 1].DisplayName} menang.";
            status.text = currentStake == 0 ? result : $"{result} Taruhan Rp{currentStake:N0} • Kembali Rp{paid:N0}.";
            household.ReportActivity(status.text);
        }

        private void Update()
        {
            if (!open) return;
            if (!household.IsDayRunning) Close();
        }

        private void Refresh()
        {
            board.text = $"{neighbors[0].DisplayName}  /  {neighbors[1].DisplayName}  /  {neighbors[2].DisplayName}\n" +
                (hasBoard ? $"Ujung meja   {left}  ·  {right}" : "Meja kosong");
            casual.interactable = !playing;
            wager.interactable = !playing && household.AvailableCash >= stake;
            pass.interactable = playing && BestTile(hands[0]) < 0;
            for (int i = 0; i < HandSize; i++)
            {
                bool show = playing && i < hands[0].Count;
                tileButtons[i].gameObject.SetActive(show);
                if (!show) continue;
                var tile = hands[0][i];
                tileLabels[i].text = $"{tile.x} | {tile.y}";
                tileButtons[i].interactable = Fits(tile);
            }
        }
    }
}
