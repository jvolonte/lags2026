using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Twins")]
    public class TwinsStickerData : StickerData
    {
        public int value;

        public override ISticker Create() => new TwinsSticker(value);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{value}", value.ToString());
    }
}