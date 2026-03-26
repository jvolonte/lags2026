using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Views
{
    public class TooltipView : MonoBehaviour
    {
        public enum Direction { Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left, TopLeft }

        [SerializeField] RectTransform viewport;
        [SerializeField] RectTransform toolTipRect;
        [SerializeField] TextMeshProUGUI tmpContent;
        [SerializeField] RectTransform content;
        [SerializeField] float defaultWorldPadding = 2f;


        public void Show (string message, Bounds item, Direction direction)
        {
            Show(message, item, direction, defaultWorldPadding);
        }
        public void Show(string message, Bounds item, Direction direction, float padding)
        {
            Vector3 worldTarget = PickPointOnBounds(item, direction, padding);
            Show(message, worldTarget);
        }
        public void Show(string message, Vector3 worldPoint)
        {
            Vector3 viewportTarget = Camera.main.WorldToViewportPoint(worldPoint);
            Vector2 screenTarget = viewport.rect.size * viewportTarget;
            Show(message, screenTarget);
        }
        public void Show(string message, Vector2 screenPos)
        {
            tmpContent.text = message;
            toolTipRect.anchoredPosition = screenPos;

            toolTipRect.localScale = Vector3.zero;
            Show();

            toolTipRect.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }

        //Not implemented yet
        //void FlipSide (CanvasSafeZone.Side sides)
        //{
        //    Vector3 anchoredPos = Vector3.zero;

        //    if (sides.HasFlag(CanvasSafeZone.Side.Left))
        //        anchoredPos.x = toolTipRect.sizeDelta.x;
        //    if (sides.HasFlag (CanvasSafeZone.Side.Right))
        //        anchoredPos.x = content.sizeDelta.x - toolTipRect.sizeDelta.x;
        //    if (sides.HasFlag (CanvasSafeZone.Side.Top))
        //        anchoredPos.y = -content.sizeDelta.y;
        //    if (sides.HasFlag (CanvasSafeZone.Side.Bottom))
        //        anchoredPos.y = toolTipRect.sizeDelta.y;

        //    content.anchoredPosition = anchoredPos;
        //}
        void Show() => toolTipRect.gameObject.SetActive(true);

        public void Hide()
        {
            toolTipRect.gameObject.SetActive(false);
            tmpContent.text = string.Empty;
        }

        Vector3 PickPointOnBounds (Bounds bounds, Direction direction, float padding)
        {
            Vector3 anchor;

            switch (direction)
            {
                case Direction.Top:
                    anchor = new Vector3(bounds.center.x, bounds.max.y + padding, bounds.center.z);
                    break;
                case Direction.TopRight:
                    anchor = new Vector3(bounds.max.x + padding * 0.5f, bounds.max.y + padding * 0.5f, bounds.center.z);
                    break;
                case Direction.Right:
                    anchor = new Vector3(bounds.max.x + padding, bounds.center.y, bounds.center.z);
                    break;
                case Direction.BottomRight:
                    anchor = new Vector3(bounds.max.x + padding * 0.5f, bounds.min.y - padding * 0.5f, bounds.center.z);
                    break;
                case Direction.Bottom:
                    anchor = new Vector3(bounds.center.x, bounds.min.y - padding, bounds.center.z);
                    break;
                case Direction.BottomLeft:
                    anchor = new Vector3(bounds.min.x - padding * 0.5f, bounds.min.y - padding * 0.5f, bounds.center.z);
                    break;
                case Direction.Left:
                    anchor = new Vector3(bounds.min.x - padding, bounds.center.y, bounds.center.z);
                    break;
                case Direction.TopLeft:
                    anchor = new Vector3(bounds.min.x - padding * 0.5f, bounds.max.y + padding * 0.5f, bounds.center.z);
                    break;
                default:
                    anchor = bounds.center;
                    break;
            }

            return anchor;
        }
    }
}