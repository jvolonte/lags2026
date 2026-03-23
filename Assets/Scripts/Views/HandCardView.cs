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
        Vector3 baseScale;
        Quaternion baseRot;
        float currentOffsetX;
        float offsetX;
        bool isHovered;

        [SerializeField] float hoverDistanceZ = .2f; 
        [SerializeField] float hoverDistanceY = .2f; 
        [SerializeField] float scale = 1.2f; 
        

        void Awake() => baseScale = transform.localScale;

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

            transform
                .DOScale(active ? baseScale * scale : baseScale, 0.2f);
            
            UpdatePosition();
        }

        public void OffsetX(float offset)
        {
            offsetX = offset;
            UpdatePosition();
        }

        void UpdatePosition()
        {
            var target = basePos + new Vector3(offsetX, 0, 0);

            if (isHovered)
            {
                target += Vector3.back * hoverDistanceZ;
                target += Vector3.up * hoverDistanceY;
            }

            transform.DOLocalMove(target, 0.2f)
                     .SetEase(Ease.OutQuad);
        }
    }
}