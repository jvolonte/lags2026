using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Golden")]
    public class GoldenStickerData : StickerData
    {
        public float value;
        [Range(0f,1f)] public float chance;

        public override ISticker Create() => new GoldenSticker(chance, value);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{value}", value.ToString())
                .Replace("{chance}", $"{chance * 100}");
    }
}