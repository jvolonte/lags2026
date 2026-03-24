using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Grace")]
    public class GraceStickerData : StickerData
    {
        [Range(0f, 1f)] public float chance = 0.5f;

        public override ISticker Create() => new GraceSticker(chance);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{chance}", $"{chance * 100}");
    }
}