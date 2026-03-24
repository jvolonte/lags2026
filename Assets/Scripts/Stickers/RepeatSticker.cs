using System.Linq;
using UnityEngine;
using Utils;

namespace Stickers
{
    public class RepeatSticker : ISticker
    {
        public int Priority => StickerPriority.Repeat;

        public float Multiplier;

        public RepeatSticker(float multiplier) => Multiplier = multiplier;

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var stickers = source.Stickers
                                 .Select(s => s.Logic)
                                 .Where(s => s is not RepeatSticker)
                                 .ToList();

            if (stickers.Count == 0)
                return;

            var reevaluated = source.CalculateWith(stickers, other, context);
            var bonus = Mathf.FloorToInt(reevaluated * Multiplier);

            var newValue = context.Value + bonus;

            context.AddStep(newValue, $"+{bonus} (Repeat)", StepType.Conditional);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
        }
    }
}