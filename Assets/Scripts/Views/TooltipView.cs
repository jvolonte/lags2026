using DG.Tweening;
using TMPro;
using UnityEngine;
using Utils;

namespace Views
{
    public class TooltipView : MonoBehaviour
    {
        [SerializeField] RectTransform viewport;
        [SerializeField] RectTransform toolTipRect;
        [SerializeField] TextMeshProUGUI tmpContent;
        [SerializeField] RectTransform content;
        [SerializeField] float defaultWorldPadding = 2f;

        public void Show(string message, Vector3 worldPoint)
        {
            var viewportTarget = Helpers.Camera.WorldToViewportPoint(worldPoint);
            var screenTarget = viewport.rect.size * viewportTarget;

            Show(message, screenTarget);

            FlipSide(viewportTarget.x >= 0.5f, viewportTarget.y <= 0.5f);
        }

        void Show(string message, Vector2 screenPos)
        {
            tmpContent.text = message;
            toolTipRect.anchoredPosition = screenPos;

            toolTipRect.localScale = Vector3.zero;
            Show();

            toolTipRect.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }

        void FlipSide(bool isRight, bool isBottom)
        {
            var pivot = new Vector2(isRight ? 1f : 0f, isBottom ? 0f : 1f);

            toolTipRect.pivot = pivot;
            content.anchoredPosition = Vector2.zero;

            var offset = 20f;

            content.anchoredPosition = new Vector2(
                isRight ? -offset : offset,
                isBottom ? offset : -offset
            );
        }

        void Show() => toolTipRect.gameObject.SetActive(true);

        public void Hide()
        {
            toolTipRect.gameObject.SetActive(false);
            tmpContent.text = string.Empty;
        }
    }
}