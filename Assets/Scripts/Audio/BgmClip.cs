using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(menuName = "Audio/BGM Clip")]
    public class BgmClip : ScriptableObject
    {
        public BgmClipId id;

        [Range(0f, 1f)] public float volumePercentage = 1f;

        public AudioClip[] variants;

        public AudioClip GetRandomVariant()
        {
            if (variants == null || variants.Length == 0)
                return null;

            return variants[Random.Range(0, variants.Length)];
        }
    }

    public enum BgmClipId
    {
        None = 0,
        Bar,
        BaltasarSoundtrack,
        CarmenSoundtrack,
        IvanSoundtrack,
    }
}