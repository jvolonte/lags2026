using System.Linq;
using Utils;

namespace Stickers
{
    public class SpiralSticker : ISticker
    {
        public int Priority => StickerPriority.Additive;

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var myType = GetType();

            int count = source.Stickers.Count(s => s.Logic.GetType() == myType);

            if (count <= 0)
                return;

            int increment = count;

            var newValue = context.Value + increment;

            context.AddStep(newValue, $"+{increment} (Spiral x{count})", StepType.Add, this);
        }

        public void ApplyRule(WinRuleSet ruleSet) { }

        public void AfterResolution(ResolutionContext context, Card source, Card other) { }
    }
}