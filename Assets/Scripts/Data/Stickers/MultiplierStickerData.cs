using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Multiplier")]
    public class MultiplierStickerData : StickerData
    {
        public float value;

        public override ISticker Create() => new MultiplierSticker(value);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{value}", value.ToString());
    }
}