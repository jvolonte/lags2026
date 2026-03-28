using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Animation
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField] Image fadeImage;

        [Header("Durations")] 
        [SerializeField] float fadeOutDuration = 0.2f;
        [SerializeField] float fadeInDuration = 0.3f;

        Tween currentTween;

        public async Task FadeOut() => 
            await FadeTo(1f, fadeOutDuration, Ease.OutQuad);

        public async Task FadeIn() => 
            await FadeTo(0f, fadeInDuration, Ease.InQuad);

        async Task FadeTo(float target, float duration, Ease ease)
        {
            KillCurrent();

            currentTween = fadeImage
                           .DOFade(target, duration)
                           .SetEase(ease)
                           .SetUpdate(true);

            await currentTween.AsyncWaitForCompletion();
        }

        void KillCurrent()
        {
            if (currentTween != null && currentTween.IsActive())
                currentTween.Kill();
        }

        public void SetInstant(float alpha)
        {
            KillCurrent();
            var c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }
    }
}