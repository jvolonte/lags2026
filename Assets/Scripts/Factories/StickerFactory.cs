using System.Numerics;
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
        readonly List<StickerData> pool;

        public StickerFactory(List<StickerData> pool)
        {
            this.pool = pool;
        }

        public (ISticker logic, StickerData data) GetRandom()
        {
            var data = pool.PickOne();
            var logic = data.Create();

            return (logic, data);
        }

        public StickerPlacement CreateRandomPlacement()
        {
            var data = pool.PickOne();
            var logic = data.Create();

            return new StickerPlacement
            {
                Logic = logic,
                Data = data,
                LocalPosition = Vector2.zero
            };
        }
    }
}