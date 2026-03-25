using System.Collections.Generic;
using System.Linq;
using Data.Stickers;

namespace Services
{
    public static class StickerEvaluationService
    {
        public static StickerData PickBestSticker(Card enemyCard, Card playerCard, List<StickerData> candidates)
        {
            if (candidates == null || candidates.Count == 0)
                return null;

            StickerData best = null;
            var bestValue = int.MinValue;

            var baseContext = new EvaluationContext { Value = enemyCard.Value };
        
            foreach (var candidate in candidates)
            {
                var simulatedStickers = enemyCard.Stickers.Select(s => s.Logic).ToList();
                simulatedStickers.Add(candidate.Create());

                var value = enemyCard.CalculateWith(simulatedStickers, playerCard, baseContext);

                if (value > bestValue)
                {
                    bestValue = value;
                    best = candidate;
                }
            }
        
            return best;
        }
    }
}