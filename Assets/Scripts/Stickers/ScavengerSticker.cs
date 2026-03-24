using Utils;

namespace Stickers
{
    public class ScavengerSticker : ISticker
    {
        public int Priority => StickerPriority.Additive;

        public int ValuePerCard;

        public ScavengerSticker(int valuePerCard = 1)
        {
            ValuePerCard = valuePerCard;
        }

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            if (context.Discard == null || context.Discard.Count == 0)
                return;

            var bonus = context.Discard.Count * ValuePerCard;
            var newValue = context.Value + bonus;

            context.AddStep(newValue, $"+{bonus} (Scavenger)", StepType.Add);
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
        }
    }
}