using Stickers;
using UnityEngine;
using Views;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Game/Sticker")]
    public abstract class StickerData : ScriptableObject
    {
        public string id;
        public StickerView prefab;
        
        public abstract ISticker Create();
    }
}