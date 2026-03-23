using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu]
    public class MultiplierStickerData : StickerData
    {
        public float value;

        public override ISticker Create() => new MultiplierSticker(value);
    }
}