using System;
using System.Collections.Generic;
using Stickers;

namespace Factories
{
    public class StickerFactory
    {
        readonly List<Func<ISticker>> pool = new()
        {
            () => new AdditiveSticker(UnityEngine.Random.Range(1, 10)),
            () => new AdditiveSticker(UnityEngine.Random.Range(1, 10)),
            () => new MultiplierSticker(1.5f),
            () => new MultiplierSticker(2f),
        };

        public ISticker GetRandom() => pool.PickOne()();
    }
}