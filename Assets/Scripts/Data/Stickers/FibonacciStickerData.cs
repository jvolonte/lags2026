using Stickers;
using UnityEngine;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Fibonacci")]
    public class FibonacciStickerData : StickerData
    {
        public override ISticker Create() => new FibonacciSticker();

        public override string GetDescription() =>
            descriptionTemplate;
    }
}