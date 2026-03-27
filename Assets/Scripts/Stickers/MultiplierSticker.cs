using UnityEngine;
using Utils;

namespace Stickers
{
    public class MultiplierSticker : ISticker
    {
        public int Priority => StickerPriority.Multiplicative;

        public float Value;

        public MultiplierSticker(float value) => Value = value;

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var newValue = Mathf.FloorToInt(context.Value * Value);
            context.AddStep(newValue, $"x{Value} (Mult)", StepType.Multiply, this);
        }

        public void ApplyRule(WinRuleSet ruleSet) { }

        public void AfterResolution(ResolutionContext context, Card source, Card other) { }

        public override string ToString() => $"Multiplier: {Value}";
    }
}