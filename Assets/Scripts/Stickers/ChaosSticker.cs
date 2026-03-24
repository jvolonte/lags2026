using UnityEngine;

namespace Stickers
{
    public class ChaosSticker : ISticker
    {
        public int Priority => 0;

        public int Value;

        public ChaosSticker(int value)
        {
            Value = value;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var roll = Random.value;
            var delta = roll <= 0.5f ? Value : -Value;

            var newValue = Mathf.Max(0, context.Value + delta);

            context.AddStep(newValue, delta > 0 ? $"+{Value}" : $"-{Value}", StepType.Conditional);
        }

        public void ApplyRule(WinRuleSet ruleSet) { }

        public void AfterResolution(ResolutionContext context, Card source, Card other) { }
    }
}