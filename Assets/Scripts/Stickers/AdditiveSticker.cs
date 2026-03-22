namespace Stickers
{
    public class AdditiveSticker : ISticker
    {
        public int Priority => 0;

        public int Value;

        public AdditiveSticker(int value) => Value = value;

        public void Resolve(Card source, Card other) => source.Evaluation += Value;

        public void ApplyRule(WinRuleSet ruleSet) {}
        public override string ToString() => $"Additive: {Value}";
    }
}