using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Chaos")]
    public class ChaosStickerData : StickerData
    {
        public int value;
        [Range(0f,1f)] public float chance;

        public override ISticker Create() => new ChaosSticker(value, chance);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{value}", value.ToString())
                .Replace("{chance}", $"{chance * 100}");
    }
}