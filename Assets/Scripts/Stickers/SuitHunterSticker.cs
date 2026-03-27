using UnityEngine;
using Utils;

namespace Stickers
{
    public class SuitHunterSticker : ISticker
    {
        public int Priority => StickerPriority.Multiplicative;
        public float Value;
        public Suit Suit;

        public SuitHunterSticker(Suit suit, float value)
        {
            Suit = suit;
            Value = value;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var newValue = other.Suit == Suit ? Mathf.FloorToInt(context.Value * Value) : context.Value;
            context.AddStep(newValue, $"*{Value}", StepType.Conditional, this);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
        }

        public override string ToString() => $"{Suit} Hunter. Applies x{Value}";
    }
}