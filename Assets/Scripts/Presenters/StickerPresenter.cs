using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Utils;
using Views;

namespace Presenters
{
    public class StickerPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Transform container;
        [SerializeField] Transform surfaceRoot;
        [SerializeField] Renderer surface;

        [Header("Layout")] [SerializeField] float spacing = 0.5f;
        [SerializeField] float surfaceOffset = 0.02f;
        [SerializeField] float depthStep = 0.01f;

        readonly List<StickerView> views = new();

        void Awake()
        {
            surfaceRoot.transform.localScale = Vector3.zero;

            CombatEventManager.OnRevealStickers += ShowSheet;
            CombatEventManager.OnClearStickers += Clear;
        }

        void OnDestroy()
        {
            CombatEventManager.OnRevealStickers -= ShowSheet;
            CombatEventManager.OnClearStickers -= Clear;
        }

        void Show(List<StickerInstance> stickers)
        {
            Clear();

            if (surface == null)
            {
                Debug.LogError("StickerPresenter: Missing MeshRenderer reference.");
                return;
            }

            var bounds = surface.bounds;

            var center = bounds.center;
            var up = surface.transform.up;
            var right = surface.transform.right;
            var normal = surface.transform.forward;

            var centerOffset = (stickers.Count - 1) * 0.5f;

            for (var i = 0; i < stickers.Count; i++)
            {
                var view = Instantiate(stickers[i].Data.prefab, container);
                var offset = (i - centerOffset) * spacing;

                var worldPos =
                    center +
                    right * 0f +
                    up * offset +
                    normal * surfaceOffset +
                    normal * (-i * depthStep);

                view.transform.position = worldPos;
                var baseRotation = Quaternion.LookRotation(-normal, up);

                var randomTilt = Quaternion.Euler(0, 0, Random.Range(-5f, 5f));
                view.transform.rotation = baseRotation * randomTilt;

                var targetScale = view.transform.localScale;
                view.transform.localScale = Vector3.zero;

                view.transform
                    .DOScale(targetScale, 0.25f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(i * 0.05f);

                view.Bind(stickers[i]);
                views.Add(view);
            }
        }

        void Clear()
        {
            container.DeleteChildren();
            views.Clear();
        }

        public void ShowSheet(List<StickerInstance> stickers)
        {
            surfaceRoot.transform.gameObject.SetActive(true);
            surfaceRoot.transform.DOScale(1f, 0.35f)
                     .SetEase(Ease.OutBack)
                     .OnComplete(() => Show(stickers));
        }

        public void HideSheet() =>
            surfaceRoot.transform.DOScale(0f, 0.35f)
                     .SetEase(Ease.InBack)
                     .OnComplete(() => surfaceRoot.transform.gameObject.SetActive(false));
    }
}