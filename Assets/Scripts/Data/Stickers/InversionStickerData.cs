using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Inversion")]
    public class InversionStickerData : StickerData
    {
        public override ISticker Create() => new InversionSticker();
        public override string GetDescription() => descriptionTemplate;
    }
}