namespace Stickers
{
    public interface ISticker
    {
        int Priority { get; }

        void Resolve(EvaluationContext context, Card source, Card other);

        void ApplyRule(WinRuleSet ruleSet);
        
        void AfterResolution(ResolutionContext context, Card source, Card other);
    }
}