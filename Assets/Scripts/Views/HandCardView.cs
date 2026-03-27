using System;
using DG.Tweening;
using Services;
using UnityEngine;
using Utils;

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

        void LateUpdate()
        {
            UpdateRotation();
        }

        public void SetCard(Card card)
        {
            Card = card;
            cardView.SetCard(card);
            cardView.AllowStickers();

            Card.OnStickerAdded += SetOverlayLayer;
        }

        void SetOverlayLayer() =>
            cardView.gameObject.ReplaceLayerRecursively(LayerService.DEFAULT_LAYER, LayerService.OVERLAY_LAYER);

        public void SetHovered(bool hovered) => OnHoverChanged?.Invoke(this, hovered);

        public void OnClicked() => CombatEventManager.PlayerPlaysCard(cardView);

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

        void UpdateRotation()
        {
            var current = transform.localRotation;

            if (!IsValidQuaternion(current))
            {
                current = Quaternion.identity;
                transform.localRotation = current;
            }

            if (!IsValidQuaternion(baseRot))
                baseRot = Quaternion.identity;

            if (isHovered)
                LookMouse();
            else
            {
                var target = Quaternion.Lerp(current, baseRot, Time.deltaTime * 10f);
                if (IsValidQuaternion(target))
                    transform.localRotation = target;
            }
        }

        void LookMouse()
        {
            if (rotationRadius <= 0.0001f)
                return;

            var cam = Helpers.Camera;

            Vector2 cardScreenPosition = cam.WorldToViewportPoint(transform.position);
            Vector2 mouseScreenPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            Vector2 delta = Vector3.ClampMagnitude(mouseScreenPosition - cardScreenPosition, rotationRadius);

            var safeRadius = Mathf.Max(rotationRadius, 0.0001f);

            var rotation = Quaternion.Euler(
                delta.y / safeRadius * rotationAngleRange,
                -delta.x / safeRadius * rotationAngleRange,
                0f
            );

            transform.localRotation = baseRot * rotation;
        }

        static bool IsValidQuaternion(Quaternion q)
        {
            if (float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w))
                return false;

            var magnitude = q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;

            return magnitude > 0.0001f;
        }

        void OnDestroy()
        {
            if (Card != null)
                Card.OnStickerAdded -= SetOverlayLayer;
        }
    }
}