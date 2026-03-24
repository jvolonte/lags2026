using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Chaos")]
    public class ChaosStickerData : StickerData
    {
        public int value;

        public override ISticker Create() => new ChaosSticker(value);
    }
}