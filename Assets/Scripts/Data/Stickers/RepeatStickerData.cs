using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Repeat")]
    public class RepeatStickerData : StickerData
    {
        [Range(0f, 1f)] public float multiplier = 0.5f;

        public override ISticker Create() => new RepeatSticker(multiplier);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{value}", $"{multiplier * 100}");
    }
}