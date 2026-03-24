using UnityEngine;
using Utils;

namespace Stickers
{
    public class FragileSticker : ISticker
    {
        public int Priority =>  StickerPriority.Multiplicative;
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
            context.AddStep(newValue, $"x{Value}", StepType.Multiply);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
            var value = Random.value;
            Debug.Log($"Rolling {value} for fragile sticker. Should destroy: {value <= BreakChance}");
            if (value <= BreakChance)
                context.Get(source).Destroy = true;
        }
    }
}