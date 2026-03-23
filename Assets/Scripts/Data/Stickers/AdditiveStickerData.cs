using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu]
    public class AdditiveStickerData : StickerData
    {
        public int value;

        public override ISticker Create() => new AdditiveSticker(value);
    }
}