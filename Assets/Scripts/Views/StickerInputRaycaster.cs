using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Views
{
    public class StickerInputRaycaster : MonoBehaviour
    {
        [SerializeField] LayerMask stickerLayer;

        Camera cam;
        StickerView currentHovered;

        void Awake()
        {
            cam = Helpers.Camera;
        }

        void OnEnable()
        {
            CombatEventManager.OnStickerDestroyed += HandleStickerInvalidated;
        }

        void OnDisable()
        {
            CombatEventManager.OnStickerDestroyed -= HandleStickerInvalidated;
        }

        void HandleStickerInvalidated(StickerView sticker)
        {
            if (sticker == currentHovered)
            {
                CombatEventManager.StickerHoverExit();
                currentHovered = null;
            }
        }

        void Update()
        {
            var newHovered = GetHoveredSticker();

            if (newHovered != currentHovered)
            {
                if (currentHovered != null) CombatEventManager.StickerHoverExit();

                if (newHovered != null)
                {
                    var col = newHovered.GetComponent<Collider>();
                    var bounds = col.bounds;

                    var pos = bounds.center + Vector3.up * bounds.extents.y;

                    var forwardToCamera = (cam.transform.position - pos).normalized;
                    var projectedForward = Vector3.ProjectOnPlane(forwardToCamera, newHovered.transform.up);
                    var rot = Quaternion.LookRotation(-projectedForward, newHovered.transform.up);

                    CombatEventManager.StickerHoverEnter(newHovered.GetData(), pos, rot);
                }

                currentHovered = newHovered;
            }
        }

        StickerView GetHoveredSticker()
        {
            if (Mouse.current == null)
                return null;

            var mousePos = Mouse.current.position.ReadValue();
            var ray = cam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out var hit, 100f, stickerLayer))
            {
                var sticker = hit.collider.GetComponent<StickerView>();

                if (sticker != null && !sticker.Dragging)
                    return sticker;
            }

            return null;
        }
    }
}