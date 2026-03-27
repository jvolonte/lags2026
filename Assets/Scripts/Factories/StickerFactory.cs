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

        public StickerPlacement CreateWithContext(StickerContext ctx)
        {
            var validPool = GetValidPool(ctx);

            var (logic, data) = validPool.Count == 0
                ? GetRandomWeighted(Pool)
                : GetRandomWeighted(validPool);

            return new StickerPlacement
            {
                Data = data,
                Logic = logic
            };
        }

        List<StickerData> GetValidPool(StickerContext ctx)
        {
            var pool = new List<StickerData>(Pool);

            if (ctx.TotalCount == 1)
                pool.RemoveAll(s => s is TwinsStickerData or MimicStickerData or RepeatStickerData);

            if (ctx.TotalCount > 0)
                pool.RemoveAll(s =>
                    s is GraceStickerData or NecromancerStickerData or ScavengerStickerData or LoneWolfStickerData);

            if (ctx.IsEven)
                pool.RemoveAll(s => s is OddStickerData);

            if (ctx.IsOdd)
                pool.RemoveAll(s => s is EvenStickerData);

            if (ctx.HasDuplicates)
                pool.RemoveAll(s => s is PurityStickerData);

            return pool;
        }
    }

    internal static class StickerRarityWeights
    {
        public static float GetWeight(StickerRarity rarity) => rarity switch
        {
            StickerRarity.Common => 65f,
            StickerRarity.Rare => 25f,
            StickerRarity.Epic => 10f,
            _ => 1f
        };
    }

    public class StickerContext
    {
        public int TotalCount { get; }
        public int CardValue { get; }

        public List<StickerData> Selected { get; } = new();

        public bool IsEven => CardValue % 2 == 0;
        public bool IsOdd => !IsEven;

        public StickerContext(int totalCount, int cardValue)
        {
            TotalCount = totalCount;
            CardValue = cardValue;
        }

        public void Register(StickerPlacement placement) =>
            Selected.Add(placement.Data);

        public bool Has(StickerData type) => Selected.Contains(type);

        public bool HasDuplicates => Selected
                                     .GroupBy(s => s.GetType())
                                     .Any(g => g.Count() > 1);
    }
}