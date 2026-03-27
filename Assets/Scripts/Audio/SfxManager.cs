using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class SfxManager : MonoBehaviour
    {
        public static SfxManager Instance { get; private set; }

        [SerializeField] AudioSource audioSourcePrefab;
        [SerializeField] int poolSize = 10;

        AudioSource[] pool;
        int currentIndex = 0;

        static Dictionary<SfxClipId, SfxClip> sfxLibrary;

        public static float Volume => 1f;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            pool = new AudioSource[poolSize];
            for (var i = 0; i < poolSize; i++)
                pool[i] = Instantiate(audioSourcePrefab, transform);

            LoadSfxLibrary();
        }

        static void LoadSfxLibrary()
        {
            sfxLibrary = new Dictionary<SfxClipId, SfxClip>();
            var clips = Resources.LoadAll<SfxClip>("Audio/SFX");

            Debug.Log($"clips loaded: {clips.Length}");
            foreach (var clip in clips) 
                sfxLibrary[clip.id] = clip;
        }

        public static void Play(SfxClipId id, float volume = 1f, float pitch = 1f)
        {
            if (Instance == null || id is SfxClipId.None) return;
            if (!sfxLibrary.TryGetValue(id, out var clip)) return;

            var selectedClip = clip.GetRandomVariant();
            if (selectedClip == null) return;

            var source = Instance.pool[Instance.currentIndex];
            source.clip = selectedClip;
            source.volume = volume * Volume * clip.volumePercentage;
            source.pitch = pitch;
            source.Play();

            Instance.currentIndex = (Instance.currentIndex + 1) % Instance.pool.Length;
        }
    }
}