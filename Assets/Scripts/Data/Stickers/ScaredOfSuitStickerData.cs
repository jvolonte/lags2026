using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Game/ScaredOfSuit")]
    public class ScaredOfSuitStickerData : StickerData
    {
        public int value;
        public Suit suit;

        public override ISticker Create() => new ScaredOfSuitSticker(suit, value);
    }
}