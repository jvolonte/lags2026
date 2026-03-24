using Utils;

namespace Stickers
{
    public class AdditiveSticker : ISticker
    {
        public int Priority => StickerPriority.Additive;

        public int Value;

        public AdditiveSticker(int value) => Value = value;

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var newValue = context.Value + Value;
            context.AddStep(newValue, $"+{Value}", StepType.Add);
        }

        public void ApplyRule(WinRuleSet ruleSet) {}
        public void AfterResolution(ResolutionContext context, Card source, Card other) { }

        public override string ToString() => $"Additive: {Value}";
    }
}