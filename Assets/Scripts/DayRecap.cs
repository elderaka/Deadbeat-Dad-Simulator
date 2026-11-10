namespace BapakSimulator
{
    public enum PickupProvider { Missed, Bapak, Ibu, PaidTransport }

    public readonly struct DayRecap
    {
        public int DayNumber { get; }
        public PickupProvider Pickup { get; }
        public int Expenses { get; }
        public int FishingMinutes { get; }
        public int Catches { get; }
        public int? FirstCastMinute { get; }

        public DayRecap(int dayNumber, PickupProvider pickup, int expenses, int fishingMinutes,
            int catches, int? firstCastMinute)
        {
            DayNumber = dayNumber;
            Pickup = pickup;
            Expenses = expenses;
            FishingMinutes = fishingMinutes;
            Catches = catches;
            FirstCastMinute = firstCastMinute;
        }
    }
}
