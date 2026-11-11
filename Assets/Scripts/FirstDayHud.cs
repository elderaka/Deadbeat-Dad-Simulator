using UnityEngine;

namespace BapakSimulator
{
    public sealed class FirstDayHud : MonoBehaviour
    {
        [SerializeField] private PickupDay day;
        [SerializeField] private ItemInventory catchBag;
        [SerializeField] private PlayerController player;
        [SerializeField] private UnityEngine.UI.Text status;
        [SerializeField] private UnityEngine.UI.Text prompt;
        [SerializeField] private bool compact;
        [SerializeField, Min(0)] private float activityNoticeSeconds;

        private int shownDay = -1;
        private int shownCash = -1;
        private int shownItems = -1;
        private int shownMinute = -1;

        private string shownObjective;
        private string shownMessage;
        private DayRecap? shownRecap;
        private string shownPrompt;
        private int shownFishingMinutes = -1;
        private float noticeUntil;
        private bool shownNotice;

        public void Configure(PickupDay scenario, PlayerController controller,
            UnityEngine.UI.Text statusText, UnityEngine.UI.Text promptText)
        {
            day = scenario;
            player = controller;
            status = statusText;
            prompt = promptText;
        }

        private void LateUpdate()
        {
            if (!day.IsConfigured) return;

            int items = catchBag != null ? catchBag.TotalCount : -1;
            if (compact && shownMessage != day.Message && day.IsDayRunning && shownDay >= 0)
                noticeUntil = Time.time + activityNoticeSeconds;
            bool showNotice = compact && Time.time < noticeUntil && !string.IsNullOrEmpty(day.Message);
            var recap = day.IsDayRunning ? null : day.LastRecap;
            if (shownItems != items || shownDay != day.DayNumber || shownCash != day.Cash || shownMinute != day.Minute ||
                shownObjective != day.Objective || shownMessage != day.Message || shownNotice != showNotice ||
                (!compact && shownFishingMinutes != day.FishingMinutes) || !System.Nullable.Equals(shownRecap, recap))
            {
                shownDay = day.DayNumber;
                shownCash = day.Cash;
                shownItems = items;
                string bag = items >= 0 ? $"  |  Tas {items}" : string.Empty;
                shownMinute = day.Minute;
                shownRecap = recap;
                shownNotice = showNotice;
                shownFishingMinutes = day.FishingMinutes;
                shownObjective = day.Objective;
                shownMessage = day.Message;
                string time = $"{shownMinute / 60:00}:{shownMinute % 60:00}";
                status.text = recap is DayRecap summary
                    ? FormatRecap(summary, day.PreviousRecap, day.RestLocationLabel)
                    : compact
                        ? $"<size=28><b>{time}</b></size>   <color=#F6CD87>HARI {Mathf.Max(1, shownDay)}</color>\n<b>Rp{shownCash:N0}</b>{bag}\n{(showNotice ? shownMessage : shownObjective)}"
                        : $"{time}  |  Sisa: {day.MinutesRemaining}m  |  Mancing: {day.FishingMinutes}m  |  Rp{shownCash}{bag}\n" +
                          $"Hari {Mathf.Max(1, shownDay)} • {shownObjective}\n{shownMessage}";
            }

            string target = player.FocusedInteractable?.Prompt;
            string binding = player.UsesTouchControls ? "Interaksi" : Application.isMobilePlatform ? "Ketuk kanan" : "E";
            string nextPrompt = string.IsNullOrEmpty(target) ? "·" : $"·\n{binding}  {target}";
            if (shownPrompt == nextPrompt) return;

            shownPrompt = nextPrompt;
            prompt.text = nextPrompt;
        }

        private static string FormatRecap(DayRecap recap, DayRecap? previous, string restLocation)
        {
            string cast = recap.FirstCastMinute is int minute ? $"{minute / 60:00}:{minute % 60:00}" : "—";
            string comparison = previous is DayRecap last
                ? $"\nHari {last.DayNumber}: {PickupLabel(last.Pickup)} • Rp{last.Expenses} • {last.FishingMinutes}m • {last.Catches} ikan"
                : string.Empty;
            return $"Hari {recap.DayNumber} selesai • Jemput: {PickupLabel(recap.Pickup)}\n" +
                $"Keluar Rp{recap.Expenses} | Mancing {recap.FishingMinutes}m | Ikan {recap.Catches}\n" +
                $"Lempar pertama: {cast}{comparison}\n{restLocation}: mulai hari berikutnya";
        }

        private static string PickupLabel(PickupProvider provider) => provider switch
        {
            PickupProvider.Bapak => "Bapak",
            PickupProvider.Ibu => "Ibu",
            PickupProvider.PaidTransport => "Antar jemput",
            _ => "Terlewat"
        };
    }
}
