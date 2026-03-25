using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Odd")]
    public class OddStickerData : StickerData
    {
        public int value;

        public override ISticker Create() => new OddSticker(value);

        public override string GetDescription()
        {
            var parity = "odd";
            var coloredParity = RichText.Colorize(parity, ColorService.Odd);
            return descriptionTemplate
                   .Replace($"{parity}", coloredParity)
                   .Replace("{value}", value.ToString());
        }
    }
}