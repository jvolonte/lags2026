using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Views
{
    public class TooltipView : MonoBehaviour
    {
        [SerializeField] TextMeshPro textMesh;
        [SerializeField] Transform background;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Vector3 offset;

        [SerializeField] float paddingX = 0.1f;
        [SerializeField] float paddingY = 0.05f;

        public void Show(string message, Vector3 worldPosition)
        {
            textMesh.text = message;
            transform.position = worldPosition + offset;

            transform.localScale = Vector3.zero;
            Show();
            transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

            StartCoroutine(UpdateNextFrame());
        }

        IEnumerator UpdateNextFrame()
        {
            yield return null;
            textMesh.ForceMeshUpdate(true, true);
            UpdateBackground();
        }

        void UpdateBackground()
        {
            var bounds = textMesh.textBounds;

            var width = bounds.size.x + paddingX;
            var height = bounds.size.y + paddingY;

            spriteRenderer.size = new Vector2(width, height);
            background.localPosition = bounds.center;
        }

        void Show() => gameObject.SetActive(true);

        public void Hide()
        {
            gameObject.SetActive(false);
            textMesh.text = string.Empty;
        }
    }
}