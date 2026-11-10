using UnityEngine;

namespace BapakSimulator
{
    [CreateAssetMenu(menuName = "Bapak Simulator/Fishing Fight Tuning")]
    public sealed class FishingFightTuning : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float runSpeed;
        [SerializeField, Min(0.1f)] private float runSeconds;
        [SerializeField, Min(0.1f)] private float recoverySeconds;
        [SerializeField, Min(0.1f)] private float staminaSeconds;
        [SerializeField, Min(0.1f)] private float dragPayoutSpeed;
        [SerializeField, Min(0.1f)] private float strainPerSecond;
        [SerializeField, Min(0.1f)] private float reliefPerSecond;
        [SerializeField, Min(0.1f)] private float landingDistance;
        [SerializeField, Min(0)] private float rodDipDegrees;

        public float RunSpeed => runSpeed;
        public float RunSeconds => runSeconds;
        public float RecoverySeconds => recoverySeconds;
        public float StaminaSeconds => staminaSeconds;
        public float DragPayoutSpeed => dragPayoutSpeed;
        public float StrainPerSecond => strainPerSecond;
        public float ReliefPerSecond => reliefPerSecond;
        public float LandingDistance => landingDistance;
        public float RodDipDegrees => rodDipDegrees;

        public bool IsValid => runSpeed > 0 && runSeconds > 0 && recoverySeconds > 0 &&
            staminaSeconds > 0 && dragPayoutSpeed > 0 && strainPerSecond > 0 &&
            reliefPerSecond > 0 && landingDistance > 0 && rodDipDegrees >= 0;
    }
}
