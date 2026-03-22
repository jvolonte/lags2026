namespace Stickers
{
    public class AdditiveSticker : ISticker
    {
        public int Priority => 0;

        public int Value;

        public AdditiveSticker(int value) => Value = value;

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var newValue = context.Value + Value;
            context.AddStep(newValue, $"+{Value}", StepType.Add);
        }

        public void ApplyRule(WinRuleSet ruleSet) {}
        
        public override string ToString() => $"Additive: {Value}";
    }
}