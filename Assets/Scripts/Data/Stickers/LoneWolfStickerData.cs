using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/LoneWolf")]
    public class LoneWolfStickerData : StickerData
    {
        public int value;

        public override ISticker Create() => new LoneWolfSticker(value);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{value}", value.ToString());
    }
}