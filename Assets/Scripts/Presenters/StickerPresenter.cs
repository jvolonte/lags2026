using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Views;

namespace Presenters
{
    public class StickerPresenter : MonoBehaviour
    {
        [SerializeField] Transform container;
        [SerializeField] float spacing = 0.5f;

        readonly List<StickerView> views = new();

        void Awake()
        {
            CombatEventManager.OnRevealStickers += Show;
            CombatEventManager.OnClearStickers += Clear;
        }

        void OnDestroy()
        {
            CombatEventManager.OnRevealStickers -= Show;
            CombatEventManager.OnClearStickers -= Clear;
        }

        void Show(List<StickerInstance> stickers)
        {
            Clear();

            var centerOffset = (stickers.Count - 1) * 0.5f;

            for (var i = 0; i < stickers.Count; i++)
            {
                var view = Instantiate(stickers[i].Data.prefab, container);

                var y = (i - centerOffset) * spacing;
                var z = -i * 0.01f;
                view.transform.localPosition = new Vector3(0, -y, z);
                view.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-5f, 5f));
                
                var scale = view.transform.localScale;
                view.transform.localScale = Vector3.zero;
                view.transform.DOScale(scale, 0.25f)
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
    }
}