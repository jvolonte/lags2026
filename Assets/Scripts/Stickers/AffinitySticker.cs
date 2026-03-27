using UnityEngine;
using Utils;

namespace Stickers
{
    public class AffinitySticker : ISticker
    {
        public int Priority => StickerPriority.Affinity;

        public Suit StrongSuit { get; private set; }
        public float StrongMultiplier { get; private set; } = 2f;

        public Suit WeakSuit { get; private set; }
        public int WeakValue { get; private set; } = 3;

        public AffinitySticker(Suit strong, Suit weak, float strongMultiplier = 2f, int weakValue = 3)
        {
            StrongSuit = strong;
            WeakSuit = weak;
            StrongMultiplier = strongMultiplier;
            WeakValue = weakValue;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var newValue = context.Value;
            var stepText = "";

            if (other.Suit == StrongSuit)
            {
                newValue = Mathf.FloorToInt(context.Value * StrongMultiplier);
                stepText = $"×{StrongMultiplier} (Affinity)";
            }
            else if (other.Suit == WeakSuit)
            {
                newValue = Mathf.Max(0, context.Value - WeakValue);
                stepText = $"-{WeakValue} (Affinity)";
            }

            if (!string.IsNullOrEmpty(stepText))
                context.AddStep(newValue, stepText, StepType.Conditional, this);
        }

        public void ApplyRule(WinRuleSet ruleSet) { }

        public void AfterResolution(ResolutionContext context, Card source, Card other) { }

        public override string ToString() => $"Affinity (Strong:{StrongSuit}, Weak:{WeakSuit})";
    }
}