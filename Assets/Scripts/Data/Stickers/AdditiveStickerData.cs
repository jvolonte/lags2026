using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Additive")]
    public class AdditiveStickerData : StickerData
    {
        public int value;

        public override ISticker Create() => new AdditiveSticker(value);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{value}", value.ToString());
    }
}