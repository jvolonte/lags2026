using UnityEngine;
using Utils;

namespace Stickers
{
    public class SunSticker : ISticker
    {
        public int Priority => StickerPriority.Multiplicative;

        public float Multiplier = 3f;

        public SunSticker(float multiplier = 3f)
        {
            Multiplier = multiplier;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var myValue = source.Value;
            var opponentValue = other.Value;

            var sameParity = myValue % 2 == opponentValue % 2;

            if (!sameParity)
                return;

            var newValue = Mathf.FloorToInt(context.Value * Multiplier);
            context.AddStep(newValue, $"x{Multiplier} (Sun)", StepType.Multiply, this);
        }

        public void ApplyRule(WinRuleSet ruleSet) { }

        public void AfterResolution(ResolutionContext context, Card source, Card other) { }
    }
}