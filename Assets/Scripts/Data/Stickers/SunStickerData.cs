using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Sun")]
    public class SunStickerData : StickerData
    {
        public float value;

        public override ISticker Create() => new SunSticker(value);

        public override string GetDescription()
        {
            var odd = "odd";
            var even = "even";
            var coloredOdd = RichText.Colorize(odd, ColorService.Odd);
            var coloredEven = RichText.Colorize(even, ColorService.Even);

            return descriptionTemplate
                   .Replace($"{odd}", coloredOdd)
                   .Replace($"{even}", coloredEven)
                   .Replace("{value}", value.ToString());
        }
    }
}