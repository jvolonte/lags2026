using Utils;

namespace Stickers
{
    public class EvenSticker : ISticker
    {
        public int Priority => StickerPriority.Additive;

        public int Value = 2;

        public EvenSticker(int value)
        {
            Value = value;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            if (source.Value % 2 != 0)
                return;

            var newValue = context.Value + Value;

            context.AddStep(newValue, $"+{Value} (Even)", StepType.Conditional);
        }

        public void ApplyRule(WinRuleSet ruleSet) { }

        public void AfterResolution(ResolutionContext context, Card source, Card other) { }
    }
}