using UnityEngine;
using UnityEngine.Events;

namespace BapakSimulator
{
    [DisallowMultipleComponent, RequireComponent(typeof(HouseholdFinances))]
    public sealed class FinanceOverdueTrigger : MonoBehaviour
    {
        private enum Watch { Any, Bill, Loan }

        [SerializeField] private Watch watch;
        [SerializeField, TextArea] private string reminder;
        [SerializeField] private UnityEvent triggered = new();

        private HouseholdFinances finances;
        private PickupDay household;
        private int lastDay;

        public string CurrentReminder => isActiveAndEnabled && IsOverdue ? reminder : string.Empty;

        private bool IsOverdue => finances != null && finances.isActiveAndEnabled &&
            ((watch != Watch.Loan && finances.BillOverdue) || (watch != Watch.Bill && finances.LoanOverdue));

        private void Awake()
        {
            finances = GetComponent<HouseholdFinances>();
            household = GetComponent<PickupDay>();
        }

        private void Update()
        {
            if (!household.IsDayRunning || household.DayNumber == lastDay || !IsOverdue) return;
            lastDay = household.DayNumber;
            if (!string.IsNullOrWhiteSpace(reminder)) household.ReportActivity(reminder);
            triggered.Invoke();
        }
    }
}
