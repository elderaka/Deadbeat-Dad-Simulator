using System;
using UnityEngine;

namespace BapakSimulator
{
    [Serializable]
    internal sealed class CheckpointData
    {
        public const int CurrentVersion = 7;

        public int version;
        public int cash;
        public bool recurringTransport;
        public RecapData latest;
        public RecapData previous;
        public Entry[] relationships;
        public Entry[] inventory;
        public string[] completedTrips;
        public VehicleData[] vehicles;
        public string[] npcFlags;
        public bool billPaid;
        public int loanOutstanding;
        public int loanDueDay;
        public string loanOfferId;
        public string difficultyId;
        public bool inheritedHistory;
        public RiskGameState[] risk;

        [Serializable]
        public struct VehicleData
        {
            public string id;
            public bool unlocked;
            public float tankLiters;
            public double odometerMeters;
            public double fuelUsedLiters;
            public double fuelPurchasedLiters;
            public long fuelSpent;

            public bool IsValid => NonnegativeFinite(tankLiters) && NonnegativeFinite(odometerMeters) &&
                NonnegativeFinite(fuelUsedLiters) && NonnegativeFinite(fuelPurchasedLiters) && fuelSpent >= 0;

            private static bool NonnegativeFinite(double value) => value >= 0 && !double.IsInfinity(value);
        }

        [Serializable]
        public struct Entry
        {
            public string id;
            public int value;

            public Entry(string id, int value)
            {
                this.id = id;
                this.value = value;
            }
        }

        [Serializable]
        public sealed class RecapData
        {
            public int day;
            public PickupProvider pickup;
            public int expenses;
            public int fishingMinutes;
            public int catches;
            public int firstCastMinute;

            public RecapData(DayRecap recap)
            {
                day = recap.DayNumber;
                pickup = recap.Pickup;
                expenses = recap.Expenses;
                fishingMinutes = recap.FishingMinutes;
                catches = recap.Catches;
                firstCastMinute = recap.FirstCastMinute ?? -1;
            }

            public bool IsValid => day > 0 && day < int.MaxValue && Enum.IsDefined(typeof(PickupProvider), pickup) &&
                expenses >= 0 && fishingMinutes >= 0 && fishingMinutes <= 24 * 60 && catches >= 0 &&
                firstCastMinute >= -1 && firstCastMinute < 24 * 60;

            public DayRecap ToRecap() => new DayRecap(day, pickup, expenses, fishingMinutes, catches,
                firstCastMinute < 0 ? null : firstCastMinute);
        }

        public static CheckpointData Read(string json)
        {
            var data = new CheckpointData { cash = -1 };
            JsonUtility.FromJsonOverwrite(json, data);
            if (data.version == 1)
            {
                data.vehicles = Array.Empty<VehicleData>();
                data.version = 2;
            }
            if (data.version == 2)
            {
                data.npcFlags = Array.Empty<string>();
                data.version = 3;
            }
            if (data.version == 3)
            {
                data.billPaid = false;
                data.loanOutstanding = 0;
                data.loanDueDay = 0;
                data.version = 4;
            }
            if (data.version == 4)
            {
                data.loanOfferId = null;
                data.version = 5;
            }
            if (data.version == 5)
            {
                data.difficultyId = null;
                data.inheritedHistory = false;
                data.version = 6;
            }
            if (data.version == 6)
            {
                data.risk = Array.Empty<RiskGameState>();
                data.version = CurrentVersion;
            }
            return data;
        }
    }
}
