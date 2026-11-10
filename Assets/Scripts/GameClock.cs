using System;
using UnityEngine;

namespace BapakSimulator
{
    public sealed class GameClock
    {
        private readonly int startMinute;
        private readonly int endMinute;
        private float minute;

        internal float CurrentMinute => minute;
        public int Minute => Mathf.FloorToInt(minute);
        public int MinutesRemaining => Mathf.CeilToInt(endMinute - minute);
        public bool HasEnded => minute >= endMinute;

        public GameClock(int startMinute, int endMinute)
        {
            if (startMinute < 0 || endMinute <= startMinute || endMinute > 24 * 60)
                throw new ArgumentOutOfRangeException(nameof(endMinute));

            this.startMinute = startMinute;
            this.endMinute = endMinute;
            Reset();
        }

        public bool CanAdvance(int minutes, int deadline) => minute + minutes <= Mathf.Min(endMinute, deadline);

        public void Reset() => minute = startMinute;

        public void Advance(float minutes)
        {
            if (float.IsNaN(minutes) || minutes < 0)
                throw new ArgumentOutOfRangeException(nameof(minutes));

            minute = Mathf.Min(endMinute, minute + minutes);
        }
    }
}
