using System.Collections;
using System.Collections.Generic;
using CardZones;
using DG.Tweening;
using Services;
using UnityEngine;

namespace Views
{
    public class HandView : MonoBehaviour
    {
        private const string OVERLAY_LAYER = "Overlay";

        [SerializeField] Transform container;
        [SerializeField] HandCardView cardPrefab;

        [SerializeField] float spacing = 0.3f;

        List<HandCardView> views = new();
        Hand hand;

        HandCardView hovered;

        [SerializeField] ViewTransitionService transitionService;
        [SerializeField] Transform deckAnchor;
        // [SerializeField] GameObject cardProxyPrefab;

        public void Bind(Hand h)
        {
            hand = h;

            h.OnCardAdded += AddCard;
            h.OnCardRemoved += RemoveCard;
        }

        void HandleHover(HandCardView view, bool isHovering)
        {
            hovered = isHovering ? view : null;
            UpdateHoverState();
        }

        void UpdateHoverState()
        {
            var hoveredIndex = hovered != null ? views.IndexOf(hovered) : -1;

            for (var i = 0; i < views.Count; i++)
            {
                var view = views[i];

                if (view == hovered)
                {
                    view.SetHover(true);
                    view.OffsetX(0);
                }
                else
                {
                    view.SetHover(false);

                    if (hoveredIndex != -1)
                    {
                        var dir = Mathf.Sign(i - hoveredIndex);
                        view.OffsetX(dir * 0.2f);
                    }
                    else
                        view.OffsetX(0);
                }
            }
        }

        void OnDestroy()
        {
            if (hand == null) return;

            hand.OnCardAdded -= AddCard;
            hand.OnCardRemoved -= RemoveCard;
        }

        void AddCard(Card card) =>
            StartCoroutine(AddCardWithTransition(card));

        IEnumerator AddCardWithTransition(Card card)
        {
            var view = Instantiate(cardPrefab, container);
            view.SetCard(card);
            view.gameObject.SetActive(false);

            view.gameObject.ReplaceLayerRecursively("Default", OVERLAY_LAYER);

            view.OnHoverChanged += HandleHover;

            views.Add(view);
            hovered = null;

            Layout();

            var (pos, rot) = ProxyPositionAndRotation(view);

            var proxy = Instantiate(view.gameObject, deckAnchor.position, deckAnchor.rotation);
            proxy.SetActive(true);
            transitionService.DisableInteractiveLayers(proxy);

            var proxyView = proxy.GetComponent<CardView>();
            proxyView?.SetCard(card);

            yield return transitionService
                         .Move(proxy.transform, pos, rot, 0.3f)
                         .WaitForCompletion();

            view.gameObject.SetActive(true);
            Destroy(proxy);

            UpdateHoverState();
            yield return new WaitForSeconds(0.05f);
        }

        (Vector3, Quaternion) ProxyPositionAndRotation(HandCardView view)
        {
            var newIndex = views.IndexOf(view);
            var centerOffset = (views.Count - 1) * 0.5f;
            var x = (newIndex - centerOffset) * spacing;
            var y = Mathf.Abs(newIndex - centerOffset) * -0.01f;
            var z = -newIndex * 0.001f;

            var targetLocalPos = new Vector3(x, y, z);
            var targetLocalRot = Quaternion.Euler(0, 0, (newIndex - centerOffset) * -3f);

            var targetWorldPos = container.TransformPoint(targetLocalPos);
            var targetWorldRot = container.rotation * targetLocalRot;

            return (targetWorldPos, targetWorldRot);
        }

        void RemoveCard(Card card)
        {
            var view = views.Find(v => v.Card == card);

            if (view == null) return;

            views.Remove(view);
            Destroy(view.gameObject);
            hovered = null;

            Layout();
            UpdateHoverState();
        }

        void Layout()
        {
            var count = views.Count;
            if (count == 0) return;

            var centerOffset = (count - 1) * 0.5f;

            for (var i = 0; i < count; i++)
            {
                var view = views[i];

                var x = (i - centerOffset) * spacing;
                var y = Mathf.Abs(i - centerOffset) * -0.01f;
                var z = -i * 0.001f;

                var targetPos = new Vector3(x, y, z);

                var targetRot = Quaternion.Euler(0, 0, (i - centerOffset) * -3f);

                view.SetBaseTransform(targetPos, targetRot);
            }
        }
    }
}