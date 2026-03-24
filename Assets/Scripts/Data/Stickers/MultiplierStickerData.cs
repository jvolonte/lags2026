using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Game/Multiplier")]
    public class MultiplierStickerData : StickerData
    {
        public float value;

        public override ISticker Create() => new MultiplierSticker(value);
    }
}