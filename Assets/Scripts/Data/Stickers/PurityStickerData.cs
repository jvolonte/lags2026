using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Purity")]
    public class PurityStickerData : StickerData
    {
        public float value;

        public override ISticker Create() => new PuritySticker(value);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{value}", value.ToString());
    }
}