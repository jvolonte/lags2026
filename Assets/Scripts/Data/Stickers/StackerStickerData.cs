using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Stacker")]
    public class StackerStickerData : StickerData
    {
        public int value;

        public override ISticker Create() => new StackerSticker(value);

        public override string GetDescription() =>
            descriptionTemplate
                .Replace("{value}", value.ToString());
    }
}