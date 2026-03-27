using Utils;

namespace Stickers
{
    public class LoneWolfSticker : ISticker
    {
        public int Priority => StickerPriority.Additive;

        public int Value;

        public LoneWolfSticker(int value = 5)
        {
            Value = value;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            if (source.Stickers.Count != 1)
                return;

            var newValue = context.Value + Value;

            context.AddStep(newValue, $"+{Value} (Lone Wolf)", StepType.Conditional, this);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
        }
    }
}