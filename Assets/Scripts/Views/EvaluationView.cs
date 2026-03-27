using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Views
{
    public class EvaluationView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] float animationDuration = 0.3f;

        public void SetValue(int value) => text.text = value.ToString();

        public Tween Play(EvaluationContext context, CardCombatView combatView)
        {
            var sequence = DOTween.Sequence();

            var currentValue = context.Steps.Count > 0
                ? context.Steps[0].PreviousValue
                : context.Value;

            text.text = currentValue.ToString();

            foreach (var step in context.Steps)
            {
                // var cardView = combatView.GetCardView();
                // if (cardView != null)
                // {
                //     var stickers = cardView.GetStickers();
                //     var stickerView = stickers.First(s => s.GetLogic().GetType() == step.Source.GetType());
                //     if (stickerView != null)
                //     {
                //         Debug.Log("not found!");
                //     }
                //     sequence.Append(HighlightSticker(stickerView));
                // }

                sequence.Append(CreateStepTween(step));
            }

            return sequence;
        }

        Tween HighlightSticker(StickerView view)
        {
            var originalScale = view.transform.localScale;
            var seq = DOTween.Sequence();

            seq.Append(view.transform.DOScale(originalScale * 1.25f, 0.1f));
            seq.Append(view.transform.DOScale(originalScale, 0.1f));

            return seq;
        }

        Tween CreateStepTween(EvaluationStep step)
        {
            var value = step.PreviousValue;

            var seq = DOTween.Sequence();

            seq.Append(
                DOTween.To(
                    () => value,
                    x =>
                    {
                        value = x;
                        text.text = x.ToString();
                    },
                    step.NewValue,
                    animationDuration
                ).SetEase(Ease.OutQuad)
            );

            seq.Join(
                transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 5, 0.5f)
            );

            seq.AppendInterval(0.05f);

            return seq;
        }
    }
}