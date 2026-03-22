using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Views
{
    public class EvaluationView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] Transform targetTransform;

        [SerializeField] float animationDuration = 0.3f;

        Queue<EvaluationStep> steps = new();
        
        public void SetValue(int value) => text.text = value.ToString();

        public void Play(EvaluationContext context)
        {
            steps.Clear();

            text.text = context.Steps.Count > 0
                ? context.Steps[0].PreviousValue.ToString()
                : context.Value.ToString();

            foreach (var step in context.Steps)
                steps.Enqueue(step);

            StopAllCoroutines();
            StartCoroutine(PlaySteps());
        }

        IEnumerator PlaySteps()
        {
            while (steps.Count > 0)
            {
                var step = steps.Dequeue();

                yield return AnimateStep(step);
            }
        }

        IEnumerator AnimateStep(EvaluationStep step)
        {
            var start = step.PreviousValue;
            var end = step.NewValue;

            yield return DOTween.To(
                                    () => start,
                                    x =>
                                    {
                                        start = x;
                                        text.text = x.ToString();
                                    },
                                    end,
                                    animationDuration
                                )
                                .SetEase(Ease.OutQuad)
                                .WaitForCompletion();

            targetTransform
                .DOPunchScale(Vector3.one * 0.2f, 0.2f, 5, 0.5f);

            yield return new WaitForSeconds(0.1f);
        }
    }
}