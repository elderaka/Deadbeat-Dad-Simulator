using UnityEngine;

namespace BapakSimulator
{
    public sealed class FinancePhoneApp : MonoBehaviour
    {
        [SerializeField] private HouseholdFinances finances;
        [SerializeField] private UnityEngine.UI.Text statement;
        [SerializeField] private UnityEngine.UI.Button billButton;
        [SerializeField] private UnityEngine.UI.Button loanButton;
        [SerializeField] private UnityEngine.UI.Text loanButtonLabel;
        [SerializeField] private UnityEngine.UI.Button cancelQuote;

        [SerializeField] private UnityEngine.UI.Button nextOffer;

        private bool confirmingLoan;
        private int selectedOffer;
        private LoanOffer Selected => finances.LoanOffers[selectedOffer];

        private void Update()
        {
            if (!finances.isActiveAndEnabled) return;
            string billStatus = finances.BillPaid ? "Lunas" : finances.BillOverdue ? "Terlambat" : "Belum dibayar";
            string loan = finances.LoanOutstanding > 0
                ? $"{finances.ActiveLender} • Sisa Rp{finances.LoanOutstanding:N0}\nHari {finances.LoanDueDay} • {(finances.LoanOverdue ? "Terlambat" : "Belum lunas")}"
                : $"{Selected.LenderName} ({selectedOffer + 1}/{finances.LoanOffers.Count})\nDana Rp{Selected.Principal:N0} • Bayar Rp{Selected.Repayment:N0}\nJatuh tempo hari {finances.QuotedDueDay(Selected)}";
            string copy = $"{finances.BillName} • Rp{finances.BillAmount:N0}\nHari {finances.BillDueDay} • {billStatus}\n\nPinjol\n{loan}";
            if (statement.text != copy) statement.text = copy;
            billButton.interactable = !confirmingLoan && finances.CanPayBill;
            loanButton.interactable = finances.LoanOutstanding > 0 ? finances.CanRepay : finances.CanBorrow(Selected);
            loanButtonLabel.text = finances.LoanOutstanding > 0 ? "Lunasi pinjaman" :
                confirmingLoan ? "Konfirmasi pinjaman" : "Ajukan Pinjol";
            bool canCompare = !confirmingLoan && finances.LoanOutstanding == 0 && finances.LoanOffers.Count > 1;
            if (nextOffer.gameObject.activeSelf != canCompare) nextOffer.gameObject.SetActive(canCompare);
            if (cancelQuote.gameObject.activeSelf != confirmingLoan)
                cancelQuote.gameObject.SetActive(confirmingLoan);
        }

        private void OnDisable() => confirmingLoan = false;

        public void PayBill() => finances.TryPayBill();

        public void LoanAction()
        {
            if (finances.LoanOutstanding > 0)
            {
                finances.TryRepay();
                return;
            }

            if (confirmingLoan)
            {
                if (finances.TryBorrow(Selected)) confirmingLoan = false;
            }
            else if (finances.CanBorrow(Selected))
                confirmingLoan = true;
        }

        public void NextOffer()
        {
            if (!confirmingLoan && finances.LoanOutstanding == 0 && finances.LoanOffers.Count > 1)
                selectedOffer = (selectedOffer + 1) % finances.LoanOffers.Count;
        }

        public void CancelQuote() => confirmingLoan = false;
    }
}
