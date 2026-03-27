using UnityEngine;
using Utils;

namespace Audio
{
    [CreateAssetMenu(menuName = "Audio/SFX Clip", fileName = "New SFX Clip")]
    public class SfxClip : ScriptableObject
    {
        public SfxClipId id;
        [Range(0,1)] public float volumePercentage = 1f;
        public AudioClip[] variants;

        public AudioClip GetRandomVariant()
        {
            if (variants == null || variants.Length == 0) return null;
            return variants.PickOne();
        }
    }

    public enum SfxClipId
    {
        None,
        Burn,
        StickerEvaluation,
        PlayCard,
        ApplySticker,
        HoverCard,
        OpponentDefeated,
        GameOver,
    }
}