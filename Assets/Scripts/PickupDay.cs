using UnityEngine;

namespace BapakSimulator
{
    [RequireComponent(typeof(PlayerProgression))]
    public sealed class PickupDay : MonoBehaviour
    {
        private enum DayState { Ready, Running, Finished }
        private enum PickupState { Pending, Completed, Missed }
        private enum Coverage { None, Ibu, Transport }

        [SerializeField] private DaySchedule schedule;
        [SerializeField] private bool startAutomatically;
        [SerializeField, Min(1)] private int transportFee = 5000;
        [SerializeField] private NpcResident familyPickupContact;

        private GameClock clock;
        private string pickupObjective;
        private DayState state;
        private PickupState pickup;
        private Coverage coverage;
        private bool recurringTransport;
        private bool manualPickupPromised;
        private bool fishingActive;
        private int dayExpenses;
        private int? firstCastMinute;

        internal event System.Action DayFinished;

        internal void RestoreCheckpoint(CheckpointData data)
        {
            LastRecap = data.latest.ToRecap();
            PreviousRecap = data.previous.day > 0 ? data.previous.ToRecap() : null;
            var recap = LastRecap.Value;
            DayNumber = recap.DayNumber;
            pickup = recap.Pickup == PickupProvider.Missed ? PickupState.Missed : PickupState.Completed;
            recurringTransport = data.recurringTransport;
            CatchesToday = recap.Catches;
            FishingMinutes = recap.FishingMinutes;
            state = DayState.Finished;
            Message = startAutomatically ? "Gunakan kasur untuk memulai hari berikutnya." : "Gunakan Rumah untuk memulai hari berikutnya.";
        }

        public int CatchesToday { get; private set; }
        public DayRecap? LastRecap { get; private set; }
        public DayRecap? PreviousRecap { get; private set; }

        public bool IsConfigured => clock != null;
        internal bool IsSchoolPickupResolved => pickup != PickupState.Pending;
        public bool IsDayRunning => state == DayState.Running;
        public int DayNumber { get; private set; }
        internal PlayerProgression Progression { get; private set; }
        public int Cash => Progression != null ? Progression.Cash : 0;
        public int AvailableCash => state == DayState.Running && pickup == PickupState.Pending &&
            coverage == Coverage.Transport ? Cash - transportFee : Cash;

        internal bool TrySpendCash(int amount)
        {
            return amount <= AvailableCash && Charge(amount);
        }

        private bool Charge(int amount)
        {
            if (!IsConfigured || !Progression.TrySpendCash(amount)) return false;
            if (IsDayRunning)
                dayExpenses = (int)System.Math.Min(int.MaxValue, (long)dayExpenses + amount);
            return true;
        }

        internal void ReportActivity(string message) => Message = message;

        internal string RestLocationLabel => startAutomatically ? "Kasur" : "Rumah";
        internal float ClockMinute => clock.CurrentMinute;
        public int Minute => clock.Minute;
        public int MinutesRemaining => clock.MinutesRemaining;
        public int FishingMinutes { get; private set; }
        public bool CanPromiseManualPickup => IsDayRunning && pickup == PickupState.Pending &&
            coverage == Coverage.None && clock.Minute < schedule.PickupDeadline;
        public bool ManualPickupPromised => CanPromiseManualPickup && manualPickupPromised;

        internal bool TryPromiseManualPickup()
        {
            if (!CanPromiseManualPickup) return false;
            manualPickupPromised = true;
            Message = "Sudah janji jemput Anak di sekolah.";
            return true;
        }

        internal int PickupStartMinute => schedule.PickupStartMinute;
        internal int PickupDeadline => schedule.PickupDeadline;
        internal bool SchoolPickupMissed => pickup == PickupState.Missed;
        internal PickupProvider SchoolPickupProvider => SchoolPickupMissed ? PickupProvider.Missed
            : state == DayState.Finished && LastRecap is DayRecap recap
            ? recap.Pickup
            : coverage switch
            {
                Coverage.Ibu => PickupProvider.Ibu,
                Coverage.Transport => PickupProvider.PaidTransport,
                _ => PickupProvider.Bapak
            };
        internal int TransportFee => transportFee;
        public bool HasRecurringTransport => recurringTransport;
        public string Message { get; private set; } = "Lihat penanda Rumah, lalu berinteraksi untuk mulai.";
        public string Objective => state switch
        {
            DayState.Ready => "Mulai di Rumah",
            DayState.Finished => pickup == PickupState.Completed ? "Hari selesai • Jemput berhasil" : "Hari selesai • Jemput terlewat",
            _ when pickup == PickupState.Completed => "Jemput selesai • Mancing atau pulang",
            _ when pickup == PickupState.Missed => "Jemput terlewat • Mancing atau pulang",
            _ when coverage == Coverage.Ibu => "Jemput dikonfirmasi • Ibu",
            _ when coverage == Coverage.Transport => "Jemput dikonfirmasi • Antar jemput",
            _ when ManualPickupPromised => "Bapak akan " + pickupObjective.ToLowerInvariant(),
            _ => pickupObjective
        };

        private void Awake()
        {
            Progression = GetComponent<PlayerProgression>();
            string error = schedule != null ? schedule.ValidationError : "Assign a Day Schedule.";
            if (error == null && (Progression == null || !Progression.HasValidStartingCash || transportFee <= 0))
                error = "Assign Player Progression with nonnegative Starting Cash and a positive Transport Fee.";
            if (error != null)
            {
                Debug.LogError($"{nameof(PickupDay)} on '{name}': {error}", this);
                enabled = false;
                return;
            }

            clock = new GameClock(schedule.StartMinute, schedule.EndMinute);
            pickupObjective = $"Jemput anak {FormatTime(schedule.PickupStartMinute)}–{FormatTime(schedule.PickupDeadline)}";
        }

        private static string FormatTime(int minute) => $"{minute / 60:00}:{minute % 60:00}";

        private void Update()
        {
            if (state == DayState.Running)
                Advance(Time.deltaTime * schedule.MinutesPerSecond);
        }

        internal void StartAfterCheckpoint()
        {
            if (startAutomatically && IsConfigured && !IsDayRunning) BeginNextDay();
        }

        public void UseHome()
        {
            if (!IsConfigured) return;
            if (state != DayState.Running)
            {
                BeginNextDay();
                return;
            }

            if (pickup == PickupState.Pending)
            {
                Message = "Jemput belum selesai. Tunggu sampai selesai sebelum pulang.";
                return;
            }

            Finish();
        }

        private void BeginNextDay()
        {
            DayNumber++;
            clock.Reset();
            pickup = PickupState.Pending;
            manualPickupPromised = false;
            FishingMinutes = 0;
            fishingActive = false;
            firstCastMinute = null;
            CatchesToday = 0;
            dayExpenses = 0;
            state = DayState.Running;
            coverage = recurringTransport && Cash >= transportFee ? Coverage.Transport : Coverage.None;
            Message = coverage == Coverage.Transport
                ? "Antar jemput terjadwal lagi hari ini."
                : recurringTransport ? "Saldo kurang untuk antar jemput hari ini." : "Jemput anak sesuai jadwal.";
        }

        public void PickUp()
        {
            if (!CanArrangePickup()) return;
            if (clock.Minute < schedule.PickupStartMinute)
            {
                Message = $"Jemput mulai pukul {FormatTime(schedule.PickupStartMinute)}.";
                return;
            }

            if (!clock.CanAdvance(schedule.PickupDuration, schedule.PickupDeadline))
            {
                Message = $"Waktu tersisa kurang dari {schedule.PickupDuration} menit untuk menjemput.";
                return;
            }

            pickup = PickupState.Completed;
            Message = "Anak sudah dijemput.";
            Advance(schedule.PickupDuration);
        }

        public void CompleteEscortedPickup()
        {
            if (!CanArrangePickup()) return;
            if (clock.Minute < schedule.PickupStartMinute)
            {
                Message = $"Jemput mulai pukul {FormatTime(schedule.PickupStartMinute)}.";
                return;
            }

            pickup = PickupState.Completed;
            Message = "Anak sudah sampai rumah.";
        }

        public void AskIbu() => TryAskIbu();

        internal bool TryAskIbu()
        {
            if (!CanArrangePickup()) return false;

            float earliest = Mathf.Max(clock.CurrentMinute, schedule.PickupStartMinute);
            if (familyPickupContact == null || !familyPickupContact.IsConfigured ||
                !familyPickupContact.Profile.HasAvailability(earliest, schedule.PickupDeadline, schedule.PickupDuration))
            {
                Message = "Ibu belum bisa menjemput karena jadwal kerja.";
                return false;
            }

            coverage = Coverage.Ibu;
            Message = "Ibu bersedia menjemput hari ini.";
            return true;
        }

        public void ArrangeTransport() => TryArrangeTransport();

        internal bool TryArrangeTransport()
        {
            if (!CanArrangePickup()) return false;
            if (Cash < transportFee)
            {
                Message = $"Saldo kurang. Biaya antar jemput Rp{transportFee}.";
                return false;
            }

            recurringTransport = true;
            coverage = Coverage.Transport;
            Message = $"Antar jemput terjadwal. Biaya Rp{transportFee} tiap kali dijemput.";
            return true;
        }

        private bool CanArrangePickup()
        {
            if (!IsConfigured) return false;
            if (state == DayState.Running && pickup == PickupState.Pending && coverage == Coverage.None)
                return true;

            Message = state != DayState.Running ? "Mulai hari di Rumah." :
                pickup == PickupState.Completed ? "Anak sudah dijemput." :
                pickup == PickupState.Missed ? "Waktu jemput sudah lewat." : "Jemput sudah dikonfirmasi.";
            return false;
        }

        internal bool TryBeginFishingCast()
        {
            if (!IsDayRunning || fishingActive) return false;

            fishingActive = true;
            firstCastMinute ??= clock.Minute;
            return true;
        }

        internal void EndFishingCast(bool caught)
        {
            if (fishingActive && caught && IsDayRunning) CatchesToday++;
            fishingActive = false;
        }

        public void Fish()
        {
            if (!IsConfigured) return;
            if (state != DayState.Running)
            {
                Message = "Mulai hari di Rumah.";
                return;
            }

            if (!clock.CanAdvance(schedule.FishingDuration, schedule.EndMinute))
            {
                Message = $"Waktu tersisa kurang dari {schedule.FishingDuration} menit untuk mancing.";
                return;
            }

            FishingMinutes += schedule.FishingDuration;
            Message = $"Bapak mancing selama {schedule.FishingDuration} menit.";
            Advance(schedule.FishingDuration);
        }

        internal bool TryUseLeisureTime(int minutes)
        {
            if (!IsDayRunning || minutes <= 0 || !clock.CanAdvance(minutes, schedule.EndMinute - 1)) return false;
            Advance(minutes);
            return true;
        }

        private void Advance(float minutes)
        {
            int before = clock.Minute;
            clock.Advance(minutes);
            if (fishingActive) FishingMinutes += clock.Minute - before;
            if (pickup == PickupState.Pending && clock.Minute >= schedule.PickupDeadline)
                ResolvePickup();
            if (clock.HasEnded) Finish();
        }

        private void ResolvePickup()
        {
            if (coverage == Coverage.None)
            {
                pickup = PickupState.Missed;
                Message = "Waktu jemput sudah lewat.";
                return;
            }

            pickup = PickupState.Completed;
            if (coverage == Coverage.Transport) Charge(transportFee);
            Message = coverage == Coverage.Transport ? $"Anak dijemput. Biaya Rp{transportFee} dibayar." : "Ibu sudah menjemput.";
        }

        private void Finish()
        {
            var provider = pickup == PickupState.Completed ? SchoolPickupProvider : PickupProvider.Missed;
            PreviousRecap = LastRecap;
            LastRecap = new DayRecap(DayNumber, provider, dayExpenses, FishingMinutes, CatchesToday, firstCastMinute);
            fishingActive = false;
            state = DayState.Finished;
            Message = startAutomatically ? "Gunakan kasur untuk memulai hari berikutnya." : "Gunakan Rumah untuk memulai hari berikutnya.";
            DayFinished?.Invoke();
        }
    }
}
