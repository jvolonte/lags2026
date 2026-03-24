using UnityEngine;

namespace Stickers
{
    public class ScaredOfSuitSticker : ISticker
    {
        public int Priority => 0;

        public Suit Suit;
        public int Value;

        public ScaredOfSuitSticker(Suit suit, int value)
        {
            Suit = suit;
            Value = value;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var newValue = other.Suit == Suit
                ? Mathf.FloorToInt(context.Value - Value)
                : context.Value;

            newValue = Mathf.Max(0, newValue);

            context.AddStep(newValue, $"-{Value}", StepType.Conditional);
        }

        public void ApplyRule(WinRuleSet ruleSet) { }
    }
}