using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Collections;
using Audio;

namespace Views
{
    public class EvaluationView : MonoBehaviour
    {
        private const string ANIMATION_TRIGGER_EVALUATION = "animEvaluationTrigger";

        [SerializeField] UnityEngine.Animation animator;
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] float animationDuration = 0.3f;

        public void SetValue(int value) => text.text = value.ToString();

        public IEnumerator Play(EvaluationContext context, CardCombatView combatView)
        {
            var currentValue = context.Steps.Count > 0
                ? context.Steps[0].PreviousValue
                : context.Value;
            text.text = currentValue.ToString();
            var cardView = combatView.GetCardView();
            var stickers = cardView.GetStickers();

            foreach (var step in context.Steps)
            {
                if (step.Source == null) continue;

                var view = stickers.First(s => s.GetLogic() == step.Source);
                if (view != null)
                {
                    yield return view.Trigger();
                }

                yield return UpdateValue(step);
            }
        }

        IEnumerator UpdateValue(EvaluationStep step)
        {
            float triggerAnimationWait = 0.08f;

            var startValue = step.PreviousValue;
            var endValue = step.NewValue;
            var value = startValue;

            float t = 0f;
            float t1 = triggerAnimationWait;

            while (t < 1f)
            {
                t = Mathf.Clamp01(t + Time.deltaTime / animationDuration);
                t1 += Time.deltaTime;

                value = Mathf.FloorToInt(Mathf.Lerp(startValue, endValue, t));

                if (value != step.PreviousValue)
                {
                    var delta = value - step.PreviousValue;
                    PlayValueSfx(delta, value);
                }

                text.text = value.ToString();

                if (t1 >= triggerAnimationWait)
                {
                    animator[ANIMATION_TRIGGER_EVALUATION].time = 0;
                    animator.Sample();
                    animator.Play(ANIMATION_TRIGGER_EVALUATION);
                    t1 -= triggerAnimationWait;
                }

                yield return null;
            }
        }

        static void PlayValueSfx(int delta, int currentValue)
        {
            if (delta == 0) return;

            var id = delta > 0
                ? SfxClipId.PositiveEvaluation
                : SfxClipId.NegativeEvaluation;

            SfxManager.Play(id, pitch: GetPitch(currentValue, delta));
        }

        static float GetPitch(int value, int delta)
        {
            var basePitch = delta > 0 ? 1f : 0.9f;
            var growth = Mathf.Log10(Mathf.Max(1, value));
            var pitch = basePitch + growth * 0.05f;
            return Mathf.Clamp(pitch, 0.8f, 1.5f);
        }
    }
}