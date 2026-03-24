using Stickers;
using UnityEngine;
using Views;

namespace Data.Stickers
{
    [CreateAssetMenu(menuName = "Stickers/Sticker")]
    public abstract class StickerData : ScriptableObject
    {
        public string id;
        [TextArea] public string descriptionTemplate;
        public StickerRarity rarity;
        public StickerView prefab;
        
        public abstract ISticker Create();
        public abstract string GetDescription();
    }
    
    public enum StickerRarity
    {
        Common,
        Rare,
        Epic
    }
}