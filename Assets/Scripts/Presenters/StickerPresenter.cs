using System.Collections.Generic;
using System.Linq;
using Data;
using DG.Tweening;
using Services;
using UnityEngine;
using Utils;
using Views;

namespace Presenters
{
    public class StickerPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        Transform container;

        [SerializeField] Transform surfaceRoot;
        [SerializeField] Renderer surface;
        [SerializeField] Transform[] slots;

        [Header("Layout")] [SerializeField] float spacing = 0.5f;
        [SerializeField] float surfaceOffset = 0.02f;
        [SerializeField] float depthStep = 0.01f;

        readonly List<StickerView> views = new();

        void Awake()
        {
            surfaceRoot.transform.localScale = Vector3.zero;

            CombatEventManager.OnRevealStickers += ShowSheet;
            CombatEventManager.OnClearStickers += Clear;
            CombatEventManager.OnEnemyPlaceStickerPreview += RemoveSticker;
        }

        void OnDestroy()
        {
            CombatEventManager.OnRevealStickers -= ShowSheet;
            CombatEventManager.OnClearStickers -= Clear;
            CombatEventManager.OnEnemyPlaceStickerPreview -= RemoveSticker;
        }

        void Show(List<StickerInstance> stickers)
        {
            Clear();

            if (surface == null)
            {
                Debug.LogError("StickerPresenter: Missing MeshRenderer reference.");
                return;
            }

            var up = surface.transform.up;
            var normal = surface.transform.forward;

            for (var i = 0; i < stickers.Count; i++)
            {
                var view = Instantiate(stickers[i].Data.prefab, container);
                view.Bind(new StickerInstance { Data = stickers[i].Data, Logic = stickers[i].Logic });

                view.transform.position = slots[i].position + normal * surfaceOffset + normal * (-i * depthStep);

                var baseRotation = Quaternion.LookRotation(-normal, up);

                var randomTilt = Quaternion.Euler(0, 0, Random.Range(-5f, 5f));
                view.transform.rotation = baseRotation * randomTilt;

                var targetScale = view.transform.localScale;
                view.transform.localScale = Vector3.zero;

                view.gameObject.ReplaceLayerRecursively(LayerService.DEFAULT_LAYER, LayerService.OVERLAY_LAYER);

                view.transform
                    .DOScale(targetScale, 0.25f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(i * 0.05f);

                view.Interactable = true;
                view.Bind(stickers[i]);
                views.Add(view);
            }
        }

        void Clear()
        {
            container.DeleteChildren();
            views.Clear();
        }

        void RemoveSticker(Card card, StickerPlacement sticker)
        {
            if (views.Any())
            {
                var stickerView = views.First(s => s.GetLogic().GetType() == sticker.Logic.GetType());
                if (stickerView != null)
                {
                    views.Remove(stickerView);
                    Destroy(stickerView.gameObject);
                }
            }
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