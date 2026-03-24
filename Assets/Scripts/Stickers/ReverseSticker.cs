namespace Stickers
{
    public class ReverseSticker : ISticker
    {
        public int Priority => 200;

        public void Resolve(EvaluationContext context, Card source, Card other) {}

        public void ApplyRule(WinRuleSet ruleSet)
        {
            ruleSet.HigherValueWins = !ruleSet.HigherValueWins;
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other) { }

        public override string ToString() => $"Reverse";
    }
}