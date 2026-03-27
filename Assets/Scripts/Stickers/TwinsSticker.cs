using System.Linq;
using Utils;

namespace Stickers
{
    public class TwinsSticker : ISticker
    {
        public int Priority => StickerPriority.Additive;

        public int Value;

        public TwinsSticker(int value = 4)
        {
            Value = value;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var myType = GetType();
            var count = source.Stickers.Count(s => s.Logic.GetType() == myType);

            if (count < 2)
                return;

            var newValue = context.Value + Value;

            context.AddStep(newValue, $"+{Value} (Twins)", StepType.Conditional, this);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
        }
    }
}