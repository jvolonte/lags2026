using System;
using DG.Tweening;
using UnityEngine;

namespace Views
{
    public class HandCardView : MonoBehaviour
    {
        [SerializeField] CardView cardView;
        public Card Card { get; private set; }

        public event Action<HandCardView, bool> OnHoverChanged;

        Vector3 basePos;
        Quaternion baseRot;
        float currentOffsetX;
        float offsetX;
        bool isHovered;

        [Range(0.02f, 0.2f)] [SerializeField] float hoverDistance = .2f; 
        
        public void SetCard(Card card)
        {
            Card = card;
            cardView.SetCard(card, false);
        }
        
        public void SetHovered(bool hovered) => OnHoverChanged?.Invoke(this, hovered);

        public void OnClicked() => CombatEventManager.PlayerPlaysCard(Card);

        public void SetBaseTransform(Vector3 pos, Quaternion rot)
        {
            basePos = pos;

            transform.DOLocalRotateQuaternion(rot, 0.25f);

            UpdatePosition();
        }

        public void SetHover(bool active)
        {
            isHovered = active;

            if (active)
                transform.SetAsLastSibling();

            transform.DOScale(active ? 1.2f : 1f, 0.2f);

            UpdatePosition();
        }

        public void OffsetX(float offset)
        {
            offsetX = offset;
            UpdatePosition();
        }

        void UpdatePosition()
        {
            transform.DOKill();
            var target = basePos + new Vector3(offsetX, 0, 0);

            if (isHovered)
            {
                var towardCamera = (Helpers.Camera.transform.position - transform.position).normalized;
                target += towardCamera * hoverDistance;
                target += Vector3.up * 0.2f;
            }

            transform.DOLocalMove(target, 0.2f).SetEase(Ease.OutQuad);
        }

        void OnMouseDown() => CombatEventManager.PlayCard(Card);
    }
}