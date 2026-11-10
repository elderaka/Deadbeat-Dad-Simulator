using UnityEngine;

namespace BapakSimulator
{
    [CreateAssetMenu(menuName = "Bapak Simulator/Fishing Tuning")]
    public sealed class FishingTuning : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float castRange;
        [SerializeField, Min(0.1f)] private float castFlightSeconds;
        [SerializeField, Min(0)] private float castArc;
        [SerializeField, Min(0)] private float biteDelay;
        [SerializeField, Min(0.1f)] private float hookWindow;
        [SerializeField, Min(0.1f)] private float reelSpeed;
        [SerializeField, Min(0.01f)] private float biteDip;
        [SerializeField, Min(0.01f)] private float hookDepth;

        [SerializeField, Min(0.1f)] private float shoreLiftRadius = 0.8f;
        [SerializeField, Min(0.01f)] private float bankClearance = 0.28f;
        [SerializeField, Min(0.1f)] private float liftSpeed = 0.95f;
        [SerializeField, Range(0.05f, 1f)] private float liftReelFactor = 0.35f;

        public float ShoreLiftRadius => shoreLiftRadius;
        public float BankClearance => bankClearance;
        public float LiftSpeed => liftSpeed;
        public float LiftReelFactor => liftReelFactor;
        public float CastRange => castRange;
        public float CastFlightSeconds => castFlightSeconds;
        public float CastArc => castArc;
        public float BiteDelay => biteDelay;
        public float HookWindow => hookWindow;
        public float ReelSpeed => reelSpeed;
        public float BiteDip => biteDip;
        public float HookDepth => hookDepth;
        public bool IsValid => castRange > 0 && castFlightSeconds > 0 && castArc >= 0 &&
            biteDelay >= 0 && hookWindow > 0 && reelSpeed > 0 && biteDip > 0 && hookDepth > 0 &&
            shoreLiftRadius > 0 && bankClearance > 0 && liftSpeed > 0 && liftReelFactor > 0;
    }
}
