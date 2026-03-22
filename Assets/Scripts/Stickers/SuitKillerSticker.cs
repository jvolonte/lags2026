using UnityEngine;

namespace Stickers
{
    public class SuitKillerSticker : ISticker
    {
        public int Priority => 150;

        public float Value;

        public Suit Suit;

        public SuitKillerSticker(Suit suit, float value) {
            Suit = suit;
            Value = value;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var newValue = other.Suit == Suit ? Mathf.FloorToInt(context.Value * Value) : context.Value;
            context.AddStep(newValue, $"*{Value}", StepType.Conditional);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public override string ToString() => $"Multiplier: {Value}";
    }
}