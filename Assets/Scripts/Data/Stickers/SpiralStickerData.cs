using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Spiral")]
    public class SpiralStickerData : StickerData
    {
        public override ISticker Create() => new SpiralSticker();

        public override string GetDescription() =>
            descriptionTemplate;
    }
}