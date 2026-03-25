using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Necromancer")]
    public class NecromancerStickerData : StickerData
    {
        public override ISticker Create() => new NecromancerSticker();

        public override string GetDescription() =>
            descriptionTemplate;
    }
}