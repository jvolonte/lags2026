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

        void Update()
        {
            if (Mouse.current == null)
                return;

            var mousePos = Mouse.current.position.ReadValue();
            var ray = cam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, stickerLayer))
            {
                var sticker = hit.collider.GetComponent<StickerView>();

                if (sticker != null && sticker != currentHovered && !sticker.Dragging)
                {
                    var bounds = hit.collider.bounds;
                    var pos = bounds.center + Vector3.up * bounds.extents.y;
                    
                    CombatEventManager.StickerHoverEnter(sticker.GetData(), pos);
                    currentHovered = sticker;
                }
            }
            else
            {
                if (currentHovered != null)
                {
                    CombatEventManager.StickerHoverExit();
                    currentHovered = null;
                }
            }
        }
    }
}