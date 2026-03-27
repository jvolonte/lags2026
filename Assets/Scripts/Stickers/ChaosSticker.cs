using UnityEngine;
using Utils;

namespace Stickers
{
    public class ChaosSticker : ISticker
    {
        public int Priority => StickerPriority.Additive;

        public int Value;
        public float Chance;

        public ChaosSticker(int value, float chance)
        {
            Value = value;
            Chance = chance;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var roll = Random.value;
            var delta = roll <= Chance ? Value : -Value;

            var newValue = Mathf.Max(0, context.Value + delta);

            context.AddStep(newValue, delta > 0
                    ? $"+{Value} (Chaos)"
                    : $"-{Value} (Chaos)",
                StepType.Conditional, this);
        }

        public void ApplyRule(WinRuleSet ruleSet) { }

        public void AfterResolution(ResolutionContext context, Card source, Card other) { }
    }
}