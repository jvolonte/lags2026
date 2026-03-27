using System.Linq;
using UnityEngine;
using Utils;

namespace Stickers
{
    public class PuritySticker : ISticker
    {
        public int Priority => StickerPriority.Multiplicative;

        public float Multiplier;

        public PuritySticker(float multiplier = 2f)
        {
            Multiplier = multiplier;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var types = source.Stickers
                              .Select(s => s.Logic.GetType())
                              .ToList();

            var uniqueCount = types.Distinct().Count();

            if (uniqueCount != types.Count)
                return;

            var newValue = Mathf.FloorToInt(context.Value * Multiplier);

            context.AddStep(newValue, $"x{Multiplier} (Purity)", StepType.Conditional, this);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
        }
    }
}