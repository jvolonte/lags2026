using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Affinity")]
    public class AffinityStickerData : StickerData
    {
        public Suit strongSuit;
        public float strongMultiplier = 2f;

        public Suit weakSuit;
        public int weakValue = 3;

        public override ISticker Create() =>
            new AffinitySticker(strongSuit, weakSuit, strongMultiplier, weakValue);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{strongMultiplier}", strongMultiplier.ToString())
                .Replace("{weakValue}", weakValue.ToString())
                .Replace("{strongSuit}", strongSuit.ToString())
                .Replace("{weakSuit}", weakSuit.ToString());
    }
}