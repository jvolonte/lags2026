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
        [SerializeField] float rotationRadius = 0.3f;
        [SerializeField] float rotationAngleRange = 20f;
        

        void Awake() => baseScale = transform.localScale;

        private void LateUpdate()
        {
            UpdateRotation();
        }

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
            baseRot = rot;

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
        void UpdateRotation ()
        {
            if (isHovered)
            {
                LookMouse();
            }
            else
            {
                transform.localRotation =
                    Quaternion.Lerp(transform.localRotation, baseRot, Time.deltaTime * 10f);
            }
        }
        void LookMouse ()
        {
            Vector2 cardScreenPosition = Camera.main.WorldToViewportPoint(transform.position);
            Vector2 mouseScreenPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Vector2 delta = Vector3.ClampMagnitude(mouseScreenPosition - cardScreenPosition, rotationRadius);

            Quaternion rotation = Quaternion.Euler(
                (delta.y/rotationRadius) * rotationAngleRange, 
                (-delta.x/rotationRadius) * rotationAngleRange, 
                0f);

            transform.localRotation = baseRot * rotation;
        }
    }
}