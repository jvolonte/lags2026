using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/SuitFear")]
    public class SuitFearStickerData : StickerData
    {
        public int value;
        public Suit suit;

        public override ISticker Create() => new SuitFearSticker(suit, value);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{value}", value.ToString())
                .Replace("{suit}", suit.ToString());
    }
}