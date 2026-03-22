using System.Collections.Generic;
using CardZones;
using UnityEngine;

namespace Views
{
    public class HandView : MonoBehaviour
    {
        [SerializeField] Transform container;
        [SerializeField] HandCardView cardPrefab;

        [SerializeField] float spacing = 0.3f;

        List<HandCardView> views = new();
        Hand hand;

        HandCardView hovered;

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

        void AddCard(Card card)
        {
            var view = Instantiate(cardPrefab, container);
            view.SetCard(card);

            view.OnHoverChanged += HandleHover;

            views.Add(view);

            Layout();
        }

        void RemoveCard(Card card)
        {
            var view = views.Find(v => v.Card == card);

            if (view == null) return;

            views.Remove(view);
            Destroy(view.gameObject);

            Layout();
        }

        void Layout()
        {
            var count = views.Count;
            if (count == 0) return;

            var centerOffset = (count - 1) * 0.5f;

            for (int i = 0; i < count; i++)
            {
                var view = views[i];

                var x = (i - centerOffset) * spacing;

                var y = Mathf.Abs(i - centerOffset) * -0.01f;

                var targetPos = new Vector3(x, y, 0);

                var targetRot = Quaternion.Euler(
                    0,
                    0,
                    (i - centerOffset) * -3f
                );

                view.SetBaseTransform(targetPos, targetRot);
            }
        }
    }
}