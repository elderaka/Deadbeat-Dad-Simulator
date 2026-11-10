using System;
using System.Collections.Generic;
using UnityEngine;

namespace BapakSimulator
{
    [DisallowMultipleComponent, RequireComponent(typeof(PickupDay))]
    public sealed class HouseholdFinances : MonoBehaviour
    {
        [SerializeField] private string billName;
        [SerializeField, Min(1)] private int billAmount;
        [SerializeField, Min(1)] private int billDueDay;
        [SerializeField] private LoanOffer[] loanOffers = Array.Empty<LoanOffer>();

        private readonly Dictionary<string, LoanOffer> offersById = new();
        private PickupDay household;

        public string BillName => billName;
        public int BillAmount => billAmount;
        public int BillDueDay => billDueDay;
        public IReadOnlyList<LoanOffer> LoanOffers => loanOffers;
        public int QuotedDueDay(LoanOffer offer) => (int)Math.Min(int.MaxValue, (long)household.DayNumber + offer.TermDays);
        internal string LoanOfferId { get; private set; }
        public string ActiveLender => LoanOfferId != null && offersById.TryGetValue(LoanOfferId, out var offer)
            ? offer.LenderName : "Pinjol";
        public bool BillPaid { get; private set; }
        public int LoanOutstanding { get; private set; }
        public int LoanDueDay { get; private set; }
        public bool BillOverdue => !BillPaid && household.DayNumber > billDueDay;
        public bool LoanOverdue => LoanOutstanding > 0 && household.DayNumber > LoanDueDay;

        public bool CanPayBill => CanTransact && !BillPaid && household.AvailableCash >= billAmount;
        public bool CanBorrow(LoanOffer offer) => CanTransact && LoanOutstanding == 0 && offer != null && offer.IsValid &&
            offersById.TryGetValue(offer.PersistentId, out var authoredOffer) && authoredOffer == offer &&
            offer.Principal <= int.MaxValue - household.Cash &&
            (long)household.DayNumber + offer.TermDays <= int.MaxValue;
        public bool CanRepay => CanTransact && LoanOutstanding > 0 && household.AvailableCash >= LoanOutstanding;

        private bool CanTransact => isActiveAndEnabled && household.IsConfigured && household.IsDayRunning;

        private void Awake()
        {
            household = GetComponent<PickupDay>();
            bool valid = !string.IsNullOrWhiteSpace(billName) && billAmount > 0 && billDueDay > 0 && loanOffers.Length > 0;
            foreach (var offer in loanOffers)
                valid &= offer != null && offer.IsValid && offersById.TryAdd(offer.PersistentId, offer);
            if (valid) return;

            Debug.LogError($"Household Finances on '{name}' needs a valid bill and uniquely identified loan offers.", this);
            enabled = false;
        }

        public bool TryPayBill()
        {
            if (!CanPayBill || !household.TrySpendCash(billAmount)) return false;
            BillPaid = true;
            household.ReportActivity($"{billName} dibayar • Rp{billAmount:N0}.");
            return true;
        }

        public bool TryBorrow(LoanOffer offer)
        {
            if (!CanBorrow(offer)) return false;
            LoanOfferId = offer.PersistentId;
            LoanOutstanding = offer.Repayment;
            LoanDueDay = QuotedDueDay(offer);
            household.Progression.CreditCash(offer.Principal);
            household.ReportActivity($"Pinjaman masuk • Rp{offer.Principal:N0}.");
            return true;
        }

        public bool TryRepay()
        {
            if (!CanRepay || !household.TrySpendCash(LoanOutstanding)) return false;
            LoanOutstanding = 0;
            LoanDueDay = 0;
            LoanOfferId = null;
            household.ReportActivity("Pinjaman lunas.");
            return true;
        }

        internal void SetInheritedLoan(int amount, int dueDay)
        {
            LoanOutstanding = amount;
            LoanDueDay = amount > 0 ? dueDay : 0;
            LoanOfferId = null;
        }

        internal void Restore(CheckpointData data)
        {
            BillPaid = data.billPaid;
            LoanOutstanding = data.loanOutstanding;
            LoanDueDay = data.loanDueDay;
            LoanOfferId = data.loanOfferId;
        }
    }
}
