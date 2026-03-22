namespace Stickers
{
    public interface ISticker
    {
        int Priority { get; }
        void Resolve(Card source, Card other);

        void ApplyRule(WinRuleSet ruleSet);
    }
}