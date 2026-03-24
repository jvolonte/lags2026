using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/SuitHunter")]
    public class SuitHunterStickerData : StickerData
    {
        public float value;
        public Suit suit;

        public override ISticker Create() => new SuitHunterSticker(suit, value);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{value}", value.ToString())
                .Replace("{suit}", suit.ToString());
    }
}