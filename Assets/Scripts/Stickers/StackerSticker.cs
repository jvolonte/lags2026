using Utils;

namespace Stickers
{
    public class StackerSticker : ISticker
    {
        public int Priority => StickerPriority.Additive;

        public int ValuePerSticker;

        public StackerSticker(int valuePerSticker = 1)
        {
            ValuePerSticker = valuePerSticker;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var count = source.Stickers.Count;
            var bonus = count * ValuePerSticker;
            var newValue = context.Value + bonus;

            context.AddStep(newValue, $"+{bonus} (Stacker)", StepType.Add, this);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
        }
    }
}