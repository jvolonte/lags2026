using System.Linq;
using Data;
using Data.Stickers;
using Stickers;
using UnityEngine;

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

        public static (ISticker logic, StickerData data) GetRandomWeighted(
            List<StickerData> stickers,
            GameContext context)
        {
            var overriddenPool = ResolveContextPool(stickers, context);
            return GetRandomWeighted(overriddenPool);
        }

        static List<StickerData> ResolveContextPool(List<StickerData> stickers, GameContext context)
        {
            var pool = new List<StickerData>();

            if (context.IsFirstOpponent)
            {
                pool = context.Round switch
                {
                    1 => stickers.Where(s => s is AdditiveStickerData or MultiplierStickerData).ToList(),
                    2 => stickers.Where(s => s is OddStickerData or EvenStickerData or StackerStickerData).ToList(),
                    _ => pool
                };
            }

            return pool.Count > 0 ? pool : stickers;
        }

        static (ISticker logic, StickerData data) GetRandomWeighted(List<StickerData> stickers)
        {
            var totalWeight = stickers.Sum(s => StickerRarityWeights.GetWeight(s.rarity));
            var roll = Random.Range(0f, totalWeight);
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
            StickerRarity.Common => 60f,
            StickerRarity.Rare => 30f,
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