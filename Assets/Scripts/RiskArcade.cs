using System;
using System.Collections.Generic;
using UnityEngine;

namespace BapakSimulator
{
    [Serializable]
    internal sealed class RiskGameState
    {
        public string id;
        public long spent;
        public long returned;
        public string lastResult;
        public int dueDay;
        public string chosen;
        public string drawn;
        public int pendingPayout;
        public int stockPrice;
        public int shares;
        public int stockDay;

        public bool IsValid => !string.IsNullOrWhiteSpace(id) && spent >= 0 && returned >= 0 &&
            dueDay >= 0 && pendingPayout >= 0 && stockPrice >= 0 && shares >= 0 && stockDay >= 0 &&
            (dueDay == 0 ? pendingPayout == 0 : !string.IsNullOrWhiteSpace(chosen) && !string.IsNullOrWhiteSpace(drawn)) &&
            (shares == 0 || stockPrice > 0);
    }

    [DisallowMultipleComponent, RequireComponent(typeof(PickupDay))]
    public sealed class RiskArcade : MonoBehaviour
    {
        [SerializeField] private RiskGameProfile[] profiles = Array.Empty<RiskGameProfile>();
        private readonly Dictionary<string, RiskGameState> states = new();
        private PickupDay household;
        private RiskGameProfile crash;
        private int crashSteps;
        private float crashElapsed;

        public IReadOnlyList<RiskGameProfile> Profiles => profiles;
        public long TotalSpent { get; private set; }
        public long TotalReturned { get; private set; }
        public bool IsCrashing => crash != null;
        public bool CanCashOut => IsCrashing && crashSteps > 0 && household.IsDayRunning;

        private void Awake()
        {
            household = GetComponent<PickupDay>();
            foreach (var profile in profiles)
            {
                if (profile != null && profile.IsValid &&
                    states.TryAdd(profile.id, new RiskGameState { id = profile.id, stockPrice = profile.stockAnchor }))
                    continue;
                Debug.LogError($"Risk arcade '{name}' needs valid profiles with unique IDs.", this);
                enabled = false;
                return;
            }
        }

        private void OnEnable() => household.DayFinished += ForfeitCrash;

        private void OnDisable()
        {
            if (household != null) household.DayFinished -= ForfeitCrash;
            ForfeitCrash();
        }

        private void Update()
        {
            if (!household.IsDayRunning)
            {
                ForfeitCrash();
                return;
            }

            foreach (var profile in profiles)
            {
                var state = states[profile.id];
                if (state.dueDay > 0 && household.DayNumber >= state.dueDay) Settle(state);
                if (profile.kind == RiskGameKind.Stock && household.DayNumber > state.stockDay)
                    MoveStock(profile, state);
            }

            if (crash == null) return;
            crashElapsed += Time.deltaTime;
            while (crash != null && crashElapsed >= crash.crashStepSeconds)
            {
                crashElapsed -= crash.crashStepSeconds;
                if (UnityEngine.Random.Range(0, 100) >= crash.crashSurvivalPercent) ForfeitCrash();
                else if (++crashSteps == crash.crashMaximumSteps) CashOut();
            }
        }

        public bool CanPlay(RiskGameProfile profile)
        {
            if (!isActiveAndEnabled || !household.IsDayRunning || crash != null || profile == null ||
                Array.IndexOf(profiles, profile) < 0 || !states.TryGetValue(profile.id, out var state))
                return false;
            return state.dueDay == 0 && (profile.kind != RiskGameKind.Stock || state.shares < int.MaxValue) &&
                household.AvailableCash >= (profile.kind == RiskGameKind.Stock ? state.stockPrice : profile.stake);
        }

        public bool Play(RiskGameProfile profile, int option)
        {
            if (!CanPlay(profile)) return false;
            var state = states[profile.id];
            if (profile.kind == RiskGameKind.Stock)
            {
                if (!Spend(state, state.stockPrice)) return false;
                state.shares++;
                state.lastResult = $"Beli 1 lot • Rp{state.stockPrice:N0}";
                return true;
            }

            bool delayed = profile.kind == RiskGameKind.Football || profile.kind == RiskGameKind.Togel;
            if (delayed && (option < 0 || option >= profile.outcomes.Length ||
                (long)household.DayNumber + profile.settlementDays > int.MaxValue)) return false;
            if (!Spend(state, profile.stake)) return false;
            if (profile.kind == RiskGameKind.Crash)
            {
                crash = profile;
                crashSteps = 0;
                crashElapsed = 0;
                state.lastResult = "Putaran dimulai.";
                return true;
            }

            var outcome = Draw(profile.outcomes);
            if (delayed)
            {
                var pick = profile.outcomes[option];
                state.chosen = pick.label;
                state.drawn = outcome.label;
                state.pendingPayout = pick.label == outcome.label ? Payout(profile.stake, pick.payoutPercent) : 0;
                state.dueDay = household.DayNumber + profile.settlementDays;
                state.lastResult = $"Tiket: {pick.label} • hasil hari {state.dueDay}";
            }
            else
            {
                int returned = Pay(state, Payout(profile.stake, outcome.payoutPercent));
                state.drawn = outcome.label;
                state.lastResult = $"Kembali Rp{returned:N0}";
            }
            return true;
        }

        private static RiskGameProfile.Outcome Draw(RiskGameProfile.Outcome[] outcomes)
        {
            int total = 0;
            foreach (var outcome in outcomes) total += outcome.weight;
            int roll = UnityEngine.Random.Range(0, total);
            foreach (var outcome in outcomes)
            {
                roll -= outcome.weight;
                if (roll < 0) return outcome;
            }
            return outcomes[outcomes.Length - 1];
        }

        private void Settle(RiskGameState state)
        {
            int returned = Pay(state, state.pendingPayout);
            state.lastResult = $"Pilihan {state.chosen} • Hasil {state.drawn}\nKembali Rp{returned:N0}";
            state.dueDay = 0;
            state.pendingPayout = 0;
        }

        private static int Payout(int amount, long percent) =>
            (int)Math.Min(int.MaxValue, amount * percent / 100);

        private long CrashMultiplier => 100L + (long)crashSteps * crash.crashStepPercent;

        public bool CashOut()
        {
            if (!CanCashOut) return false;
            var state = states[crash.id];
            int returned = Pay(state, Payout(crash.stake, CrashMultiplier));
            state.lastResult = $"Cair pada {CrashMultiplier / 100f:0.00}× • Rp{returned:N0}";
            crash = null;
            return true;
        }

        public void ForfeitCrash()
        {
            if (crash == null) return;
            states[crash.id].lastResult = $"Putaran berakhir • −Rp{crash.stake:N0}";
            crash = null;
        }

        private void MoveStock(RiskGameProfile profile, RiskGameState state)
        {
            if (state.stockDay > 0)
            {
                double noise = UnityEngine.Random.Range(-profile.stockVolatility, profile.stockVolatility);
                if (UnityEngine.Random.value < profile.stockBadNewsChance) noise -= profile.stockBadNewsDrop;
                double next = state.stockPrice + (profile.stockAnchor - (double)state.stockPrice) * profile.stockReversion +
                    state.stockPrice * noise;
                state.stockPrice = (int)Math.Clamp(Math.Round(next), 1, int.MaxValue);
            }
            state.stockDay = household.DayNumber;
        }

        public bool CanSell(RiskGameProfile profile) =>
            household.IsDayRunning && profile.kind == RiskGameKind.Stock && states[profile.id].shares > 0 &&
            SellQuote(profile, states[profile.id]) <= int.MaxValue - household.Cash;

        private static int SellQuote(RiskGameProfile profile, RiskGameState state) =>
            Payout(state.stockPrice, 100 - profile.stockSpreadPercent);

        public bool Sell(RiskGameProfile profile)
        {
            if (!CanSell(profile)) return false;
            var state = states[profile.id];
            state.shares--;
            int returned = Pay(state, SellQuote(profile, state));
            state.lastResult = $"Jual 1 lot • Rp{returned:N0}";
            return true;
        }

        private bool Spend(RiskGameState state, int amount)
        {
            if (!household.TrySpendCash(amount)) return false;
            state.spent = SaturatingAdd(state.spent, amount);
            TotalSpent = SaturatingAdd(TotalSpent, amount);
            return true;
        }

        private int Pay(RiskGameState state, int amount)
        {
            int paid = Mathf.Min(amount, int.MaxValue - household.Cash);
            household.Progression.CreditCash(paid);
            state.returned = SaturatingAdd(state.returned, paid);
            TotalReturned = SaturatingAdd(TotalReturned, paid);
            return paid;
        }

        private static long SaturatingAdd(long a, long b) => a > long.MaxValue - b ? long.MaxValue : a + b;

        public string GetSlotResult(RiskGameProfile profile) => states[profile.id].drawn;

        public string GetCrashDisplay(RiskGameProfile profile) =>
            crash == profile ? $"{CrashMultiplier / 100f:0.00}×" : "1.00×";

        public int GetStockPrice(RiskGameProfile profile) => states[profile.id].stockPrice;

        public string GetStatus(RiskGameProfile profile)
        {
            var state = states[profile.id];
            if (crash == profile) return "Ambil hasil sebelum jatuh.";
            string detail = profile.kind == RiskGameKind.Stock
                ? $"Beli Rp{state.stockPrice:N0} • Jual Rp{SellQuote(profile, state):N0}\nDimiliki: {state.shares} lot"
                : $"Taruhan Rp{profile.stake:N0}";
            return $"{detail}\n{state.lastResult}\nArus kas bersih: Rp{state.returned - state.spent:N0}";
        }

        internal RiskGameState[] Capture() => new List<RiskGameState>(states.Values).ToArray();

        internal void Restore(RiskGameState[] saved)
        {
            crash = null;
            TotalSpent = TotalReturned = 0;
            foreach (var profile in profiles)
                states[profile.id] = new RiskGameState { id = profile.id, stockPrice = profile.stockAnchor };
            foreach (var state in saved)
            {
                states[state.id] = state;
                TotalSpent = SaturatingAdd(TotalSpent, state.spent);
                TotalReturned = SaturatingAdd(TotalReturned, state.returned);
            }
        }
    }
}
