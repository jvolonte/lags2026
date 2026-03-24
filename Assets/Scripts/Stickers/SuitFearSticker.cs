using UnityEngine;
using Utils;

namespace Stickers
{
    public class SuitFearSticker : ISticker
    {
        public int Priority => StickerPriority.Additive;

        public Suit Suit;
        public int Value;

        public SuitFearSticker(Suit suit, int value)
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

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
        }
    }
}