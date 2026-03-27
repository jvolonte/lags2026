using UnityEngine;
using Utils;

namespace Stickers
{
    public class UnderdogSticker : ISticker
    {
        public int Priority => StickerPriority.Multiplicative;

        public float Multiplier;

        public UnderdogSticker(float multiplier = 2f)
        {
            Multiplier = multiplier;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            if (other.Value <= source.Value)
                return;

            var newValue = Mathf.FloorToInt(context.Value * Multiplier);

            context.AddStep(newValue, $"x{Multiplier} (Underdog)", StepType.Conditional, this);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
        }
    }
}