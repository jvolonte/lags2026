using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Fragile")]
    public class FragileStickerData : StickerData
    {
        public float value;
        [Range(0f,1f)] public float breakChance = 0.25f;

        public override ISticker Create() => new FragileSticker(value, breakChance);
    }
}