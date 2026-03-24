using UnityEngine;

namespace Stickers
{
    public class GraceSticker : ISticker
    {
        public int Priority => 0;

        public float TriggerChance;
        public GraceSticker(float triggerChance) => TriggerChance = triggerChance;

        public void Resolve(EvaluationContext context, Card source, Card other) { }

        public void ApplyRule(WinRuleSet ruleSet) { }

        public void AfterResolution(ResolutionContext ctx, Card source, Card other)
        {
            var outcome = ctx.Get(source);

            if (!outcome.WillBeLost)
                return;

            var value = Random.value;
            Debug.Log($"Rolling {value} for Grace sticker. Should trigger: {value <= TriggerChance}");
            
            if (value <= TriggerChance) 
                outcome.WillBeLost = false;
        }
    }
}