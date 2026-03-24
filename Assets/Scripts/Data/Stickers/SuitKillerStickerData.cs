using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Game/SuitKiller")]
    public class SuitKillerStickerData : StickerData
    {
        public float value;
        public Suit suit;

        public override ISticker Create() => new SuitKillerSticker(suit, value);
    }
}