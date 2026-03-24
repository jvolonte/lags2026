using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Underdog")]
    public class UnderdogStickerData : StickerData
    {
        public float value;

        public override ISticker Create() => new UnderdogSticker(value);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{value}", value.ToString());
    }
}