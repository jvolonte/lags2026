using Data.Stickers;
using Stickers;

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
        
    }
}