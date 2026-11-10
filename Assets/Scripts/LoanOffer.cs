using UnityEngine;

namespace BapakSimulator
{
    [CreateAssetMenu(menuName = "Bapak Simulator/Economy/Loan Offer")]
    public sealed class LoanOffer : ScriptableObject
    {
        [SerializeField] private string persistentId;
        [SerializeField] private string lenderName;
        [SerializeField, Min(1)] private int principal;
        [SerializeField, Min(1)] private int repayment;
        [SerializeField, Min(1)] private int termDays;

        public string LenderName => lenderName;
        public int Principal => principal;
        public int Repayment => repayment;
        public int TermDays => termDays;
        internal string PersistentId => persistentId;
        internal bool IsValid => !string.IsNullOrWhiteSpace(persistentId) && !string.IsNullOrWhiteSpace(lenderName) &&
            principal > 0 && repayment > principal && termDays > 0;
    }
}
