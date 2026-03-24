using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Scavenger")]
    public class ScavengerStickerData : StickerData
    {
        public int value;

        public override ISticker Create() => new ScavengerSticker(value);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{value}", value.ToString());
    }
}