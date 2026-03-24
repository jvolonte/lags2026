using System.Linq;
using Utils;

namespace Stickers
{
    public class FibonacciSticker : ISticker
    {
        public int Priority => StickerPriority.Additive;

        public void Resolve(EvaluationContext context, Card source, Card other)
        {
            var myType = GetType();

            var sameType = source.Stickers
                                 .Where(s => s.Logic.GetType() == myType)
                                 .Select(s => s.Logic)
                                 .ToList();

            var index = sameType.IndexOf(this) + 1;

            if (index <= 0)
                return;

            var increment = GetFibonacci(index);

            var newValue = context.Value + increment;

            context.AddStep(newValue, $"+{increment} (Fib {index})", StepType.Add);
        }

        int GetFibonacci(int n)
        {
            if (n <= 0) return 0;
            if (n == 1 || n == 2) return 1;

            int a = 1;
            int b = 1;

            for (int i = 3; i <= n; i++)
            {
                var next = a + b;
                a = b;
                b = next;
            }

            return b;
        }

        public void ApplyRule(WinRuleSet ruleSet)
        {
        }

        public void AfterResolution(ResolutionContext context, Card source, Card other)
        {
        }
    }
}