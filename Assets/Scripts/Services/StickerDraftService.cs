using System.Collections.Generic;
using Data.Stickers;
using Factories;

namespace Services
{
    public class StickerDraftService
    {
        readonly StickerFactory factory;

        public StickerDraftService(StickerFactory factory)
        {
            this.factory = factory;
        }

        public List<StickerInstance> Draft(int count)
        {
            var pool = new List<StickerData>(factory.Pool);
            var result = new List<StickerInstance>();

            for (var i = 0; i < count; i++)
            {
                var (logic, data) = StickerFactory.GetRandomWeighted(pool);
                result.Add(new StickerInstance { Logic = logic, Data = data });
                pool.RemoveAll(s => s.GetType() == data.GetType());
            }

            return result;
        }
    }
}