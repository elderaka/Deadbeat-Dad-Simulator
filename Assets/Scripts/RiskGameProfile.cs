using System;
using UnityEngine;

namespace BapakSimulator
{
    public enum RiskGameKind { Slots, Crash, Football, Togel, Stock }

    [CreateAssetMenu(menuName = "Bapak Simulator/Economy/Risk Game")]
    public sealed class RiskGameProfile : ScriptableObject
    {
        [Serializable]
        public struct Outcome
        {
            public string label;
            [Min(1)] public int weight;
            [Min(0)] public int payoutPercent;
        }

        public string id;
        public string displayName;
        [TextArea] public string banner;
        public RiskGameKind kind;
        [Min(1)] public int stake;
        public Outcome[] outcomes = Array.Empty<Outcome>();
        [Min(1)] public int settlementDays;
        [Min(.1f)] public float crashStepSeconds;
        [Min(1)] public int crashStepPercent;
        [Range(1, 99)] public int crashSurvivalPercent;
        [Range(1, 100)] public int crashMaximumSteps;
        [Min(1)] public int stockAnchor;
        [Range(0, 1)] public float stockReversion;
        [Range(0, 1)] public float stockVolatility;
        [Range(0, 1)] public float stockBadNewsChance;
        [Range(0, 1)] public float stockBadNewsDrop;
        [Range(0, 99)] public int stockSpreadPercent;

        internal bool IsValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(displayName)) return false;
                if (kind == RiskGameKind.Stock) return stockAnchor > 0 && stockSpreadPercent >= 0 && stockSpreadPercent < 100;
                if (stake <= 0) return false;
                if (kind == RiskGameKind.Crash)
                    return crashStepSeconds > 0 && crashStepPercent > 0 && crashStepPercent <= 10000 && crashSurvivalPercent > 0 &&
                        crashSurvivalPercent < 100 && crashMaximumSteps > 0 && crashMaximumSteps <= 100;
                long total = 0;
                foreach (var outcome in outcomes)
                {
                    if (string.IsNullOrWhiteSpace(outcome.label) || outcome.weight <= 0 || outcome.payoutPercent < 0)
                        return false;
                    total += outcome.weight;
                }
                return outcomes.Length > 1 && total <= int.MaxValue && (kind == RiskGameKind.Slots || settlementDays > 0);
            }
        }
    }
}
