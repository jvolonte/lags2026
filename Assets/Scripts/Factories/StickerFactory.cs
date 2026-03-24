using System.Linq;
using Data;
using Data.Stickers;
using Stickers;
using Utils;
using Vector2 = UnityEngine.Vector2;

namespace Factories
{
    using System.Collections.Generic;

    public class StickerFactory
    {
        public readonly List<StickerData> Pool;

        public StickerFactory(List<StickerData> pool)
        {
            Pool = pool;
        }

        public (ISticker logic, StickerData data) GetRandom()
        {
            var data = Pool.PickOne();
            var logic = data.Create();

            return (logic, data);
        }

        public StickerPlacement CreateRandomPlacement()
        {
            var data = Pool.PickOne();
            var logic = data.Create();

            return new StickerPlacement
            {
                Logic = logic,
                Data = data,
                LocalPosition = Vector2.zero
            };
        }

        public static (ISticker logic, StickerData data) GetRandomWeighted(List<StickerData> stickers)
        {
            var totalWeight = stickers.Sum(s => StickerRarityWeights.GetWeight(s.rarity));
            var roll = UnityEngine.Random.Range(0f, totalWeight);
            var cumulative = 0f;

            foreach (var data in stickers)
            {
                cumulative += StickerRarityWeights.GetWeight(data.rarity);

                if (roll <= cumulative)
                    return (data.Create(), data);
            }

            var fallback = stickers[^1];
            return (fallback.Create(), fallback);
        }
    }

    internal static class StickerRarityWeights
    {
        public static float GetWeight(StickerRarity rarity) => rarity switch
        {
            StickerRarity.Common => 70f,
            StickerRarity.Rare => 25f,
            StickerRarity.Epic => 5f,
            _ => 1f
        };
    }
}