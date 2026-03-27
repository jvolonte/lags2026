using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class BgmManager : MonoBehaviour
    {
        public static BgmManager Instance { get; private set; }

        [SerializeField] AudioSource sourceA;
        [SerializeField] AudioSource sourceB;
        [SerializeField] float defaultFadeDuration = 1.5f;

        AudioSource current;
        AudioSource next;

        Coroutine fadeRoutine;

        static Dictionary<BgmClipId, BgmClip> bgmLibrary;

        public static float Volume => 1f;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            current = sourceA;
            next = sourceB;

            current.volume = 0f;
            next.volume = 0f;

            LoadLibrary();
        }

        static void LoadLibrary()
        {
            bgmLibrary = new Dictionary<BgmClipId, BgmClip>();
            var clips = Resources.LoadAll<BgmClip>("Audio/BGM");

            Debug.Log($"BGM loaded: {clips.Length}");

            foreach (var clip in clips)
                bgmLibrary[clip.id] = clip;
        }

        public static void Play(BgmClipId id, float fadeDuration = -1f)
        {
            if (Instance == null || id == BgmClipId.None) return;
            if (!bgmLibrary.TryGetValue(id, out var clip)) return;

            var selected = clip.GetRandomVariant();
            if (selected == null) return;

            Instance.InternalPlay(selected, clip.volumePercentage, fadeDuration);
        }

        void InternalPlay(AudioClip clip, float volumeMultiplier, float fadeDuration)
        {
            if (fadeDuration < 0f)
                fadeDuration = defaultFadeDuration;

            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);

            next.clip = clip;
            next.loop = true;
            next.volume = 0f;
            next.Play();

            float targetVolume = Volume * volumeMultiplier;

            fadeRoutine = StartCoroutine(
                Crossfade(current, next, fadeDuration, targetVolume)
            );

            (current, next) = (next, current);
        }

        IEnumerator Crossfade(AudioSource from, AudioSource to, float duration, float targetVolume)
        {
            float time = 0f;
            float startFromVolume = from.volume;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                to.volume = Mathf.Lerp(0f, targetVolume, t);
                from.volume = Mathf.Lerp(startFromVolume, 0f, t);

                yield return null;
            }

            to.volume = targetVolume;

            from.Stop();
            from.clip = null;
        }

        public static void Stop(float fadeDuration = 1f)
        {
            if (Instance == null) return;

            if (Instance.fadeRoutine != null)
                Instance.StopCoroutine(Instance.fadeRoutine);

            Instance.fadeRoutine = Instance.StartCoroutine(
                Instance.FadeOut(Instance.current, fadeDuration)
            );
        }

        IEnumerator FadeOut(AudioSource source, float duration)
        {
            float time = 0f;
            float startVolume = source.volume;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                source.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            source.Stop();
            source.clip = null;
        }
    }
}