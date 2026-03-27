using System.Linq;
using UnityEngine;
using Utils;

namespace Stickers
{
    public class MimicSticker : ISticker
    {
        public int Priority => StickerPriority.Mimic;

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var candidates = source.Stickers
                                   .Select(s => s.Logic)
                                   .Where(s => s is not MimicSticker)
                                   .ToList();

            if (candidates.Count == 0)
                return;

            var chosen = candidates[Random.Range(0, candidates.Count)];
            Debug.Log($"Mimic copying {chosen.GetType().Name}");
            chosen.Resolve(context, source, other);

            context.AddStep(context.Value, $"Mimic → {chosen.GetType().Name}", StepType.Conditional, this);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
        }
    }
}