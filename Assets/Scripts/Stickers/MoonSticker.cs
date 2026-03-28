using UnityEngine;
using Utils;

namespace Stickers
{
    public class MoonSticker : ISticker
    {
        public int Priority => StickerPriority.Multiplicative;

        public float Multiplier = 3f;

        public MoonSticker(float multiplier = 3f)
        {
            Multiplier = multiplier;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var myValue = source.Value;
            var opponentValue = other.Value;

            var oppositeParity = myValue % 2 != opponentValue % 2;

            if (!oppositeParity)
                return;

            var newValue = Mathf.FloorToInt(context.Value * Multiplier);
            context.AddStep(newValue, $"x{Multiplier} (Moon)", StepType.Multiply, this);
        }

        public void ApplyRule(WinRuleSet ruleSet) { }

        public void AfterResolution(ResolutionContext context, Card source, Card other) { }
    }
}