using System.Collections.Generic;
using Data.Stickers;
using Factories;
using Presenters;
using UnityEngine;

namespace Services
{
    public class StickerDraftService
    {
        readonly StickerFactory factory;

        public StickerDraftService(StickerFactory factory)
        {
            this.factory = factory;
        }

        public List<StickerInstance> Draft(int count, GameContext context)
        {
            var pool = new List<StickerData>(factory.Pool);
            var result = new List<StickerInstance>();

            for (var i = 0; i < count; i++)
            {
                var (logic, data) = StickerFactory.GetRandomWeighted(pool, context);
                result.Add(new StickerInstance { Logic = logic, Data = data });

                if (IsTutorialRound(context))
                    pool.RemoveAll(s => IsSameVariant(s, data));
                else
                    pool.RemoveAll(s => s.GetType() == data.GetType());
            }

            return result;
        }

        static bool IsSameVariant(StickerData a, StickerData b)
        {
            if (a.GetType() != b.GetType())
                return false;

            return a switch
            {
                AdditiveStickerData x when b is AdditiveStickerData y => x.value == y.value,
                MultiplierStickerData x when b is MultiplierStickerData y => Mathf.Approximately(x.value, y.value),
                _ => false
            };
        }

        static bool IsTutorialRound(GameContext context) =>
            context.Round == 1 && context.Enemy.Data.id == EnemyId.Alfonso;
    }
}