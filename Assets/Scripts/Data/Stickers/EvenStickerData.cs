using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Even")]
    public class EvenStickerData : StickerData
    {
        public int value;

        public override ISticker Create() => new EvenSticker(value);

        public override string GetDescription()
        {
            var parity = "even";
            var coloredParity = RichText.Colorize(parity, ColorService.Even);
            return descriptionTemplate
                   .Replace($"{parity}", coloredParity)
                   .Replace("{value}", value.ToString());
        }
    }
}