using UnityEngine;
using Utils;

namespace Stickers
{
    public class FragileSticker : ISticker
    {
        public int Priority => StickerPriority.Multiplicative;
        public float Value;
        public float BreakChance = 0.25f;

        public FragileSticker(float value, float breakChance)
        {
            Value = value;
            BreakChance = breakChance;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var newValue = Mathf.FloorToInt(context.Value * Value);
            context.AddStep(newValue, $"x{Value} (Fragile)", StepType.Multiply, this);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
            if (Random.value <= BreakChance)
            {
                context.Get(source).Destroy = true;
                Debug.Log($"Fragile destroyed card");
            }
        }
    }
}