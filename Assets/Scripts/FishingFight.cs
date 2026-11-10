using UnityEngine;

namespace BapakSimulator
{
    public sealed class FishingFight
    {
        private readonly FishingFightTuning tuning;
        private float stamina;
        private float phaseRemaining;
        private bool running = true;

        public float LineLength { get; private set; }
        public float Tension { get; private set; }
        public bool IsRunning => running && stamina > 0;
        public bool HasSnapped => Tension >= 1f;
        public int RunDirection { get; private set; } = 1;

        public FishingFight(FishingFightTuning tuning, float initialLength)
        {
            this.tuning = tuning;
            LineLength = Mathf.Max(tuning.LandingDistance * 0.5f, initialLength);
            stamina = tuning.StaminaSeconds;
            phaseRemaining = tuning.RunSeconds;
        }

        public void Advance(float deltaTime, bool reeling, float fishDistance, float reelSpeed)
        {
            if (reeling)
                LineLength = Mathf.Max(tuning.LandingDistance * 0.5f, LineLength - reelSpeed * deltaTime);
            else if (IsRunning && fishDistance > LineLength)
                LineLength = Mathf.MoveTowards(LineLength, fishDistance, tuning.DragPayoutSpeed * deltaTime);

            bool taut = fishDistance >= LineLength - 0.05f;
            float targetTension = !taut ? 0.08f :
                IsRunning ? (reeling ? 1f : 0.35f) : (reeling ? 0.42f : 0.18f);
            float rate = targetTension > Tension ? tuning.StrainPerSecond : tuning.ReliefPerSecond;
            Tension = Mathf.MoveTowards(Tension, targetTension, rate * deltaTime);

            if (stamina <= 0) return;

            if (running) stamina = Mathf.Max(0, stamina - deltaTime);
            phaseRemaining -= deltaTime;
            if (phaseRemaining > 0) return;

            running = !running;
            phaseRemaining = running ? tuning.RunSeconds : tuning.RecoverySeconds;
            if (running) RunDirection = -RunDirection;
        }
    }
}
