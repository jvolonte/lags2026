using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Mimic")]
    public class MimicStickerData : StickerData
    {
        public override ISticker Create() => new MimicSticker();

        public override string GetDescription() => descriptionTemplate;
    }
}