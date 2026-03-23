using Data.Stickers;
using Stickers;
using UnityEngine;

namespace Data
{
    public class StickerPlacement
    {
        public ISticker Logic;
        public StickerData Data;
        public Vector2 LocalPosition;

        public override string ToString() => $"{Logic} at {LocalPosition}";
    }
}