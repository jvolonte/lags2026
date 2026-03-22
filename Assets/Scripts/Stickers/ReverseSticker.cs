using UnityEngine;

namespace Stickers
{
    public class ReverseSticker : ISticker
    {
        public int Priority => 200;

        public void Resolve(Card source, Card other) {}

        public void ApplyRule(WinRuleSet ruleSet)
        {
            ruleSet.GreaterValueWins = !ruleSet.GreaterValueWins;
        }
    }
}