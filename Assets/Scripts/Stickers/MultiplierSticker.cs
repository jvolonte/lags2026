using UnityEngine;

namespace Stickers
{
    public class MultiplierSticker : ISticker
    {
        public int Priority => 100;

        public float Value;

        public MultiplierSticker(float value) => Value = value;

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var newValue = Mathf.FloorToInt(context.Value * Value);
            context.AddStep(newValue, $"*{Value}", StepType.Multiply);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other) { }

        public override string ToString() => $"Multiplier: {Value}";
    }
}