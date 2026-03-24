namespace Stickers
{
    public class NecroSticker : ISticker
    {
        public int Priority => 10;

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            if (context.Discard == null || context.Discard.Count == 0)
                return;

            var topCard = context.Discard.Peek();
            var value = topCard.Value;

            var newValue = context.Value + value;

            context.AddStep(newValue, $"+{value} (Necro)", StepType.Add);
        }

        public void ApplyRule(WinRuleSet ruleSet) { }

        public void AfterResolution(ResolutionContext context, Card source, Card other) { }
    }
}