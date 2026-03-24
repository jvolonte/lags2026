using UnityEngine;
using Utils;

namespace Stickers
{
    public class GoldenSticker : ISticker
    {
        public int Priority => StickerPriority.Multiplicative;

        public float TriggerChance = 0.2f;
        public float Multiplier = 3f;

        public GoldenSticker(float chance = 0.2f, float multiplier = 3f)
        {
            TriggerChance = chance;
            Multiplier = multiplier;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var roll = Random.value;

            Debug.Log($"Golden roll: {roll} (trigger <= {TriggerChance})");

            if (roll > TriggerChance)
                return;

            var newValue = Mathf.FloorToInt(context.Value * Multiplier);

            context.AddStep(newValue, $"x{Multiplier} (Golden)", StepType.Multiply);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
        }
    }
}