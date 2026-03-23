using UnityEngine;
using UnityEngine.InputSystem;
using Views;

public class StickerDragHandler : MonoBehaviour
{
    Camera cam;

    StickerView dragging;
    Vector3 offset;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
            TryStartDrag(mouse.position.ReadValue());

        if (mouse.leftButton.isPressed && dragging != null)
            Drag(mouse.position.ReadValue());

        if (mouse.leftButton.wasReleasedThisFrame && dragging != null)
            EndDrag(mouse.position.ReadValue());
    }

    void TryStartDrag(Vector2 screenPos)
    {
        var ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out var hit))
        {
            var view = hit.collider.GetComponentInParent<StickerView>();
            if (view == null) return;

            dragging = view;

            offset = dragging.transform.position - hit.point;
        }
    }

    void Drag(Vector2 screenPos)
    {
        var ray = cam.ScreenPointToRay(screenPos);

        var plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out var dist))
        {
            var point = ray.GetPoint(dist);
            dragging.transform.position = point + offset;
        }
    }

    void EndDrag(Vector2 screenPos)
    {
        var ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out var hit))
        {
            var card = hit.collider.GetComponentInParent<CardView>();

            if (card != null) 
                ApplySticker(card, dragging);
        }

        dragging = null;
    }

    void ApplySticker(CardView cardView, StickerView stickerView)
    {
        var card = cardView.GetCard();
        var sticker = stickerView.GetLogic();

        card.Stickers.Add(sticker);

        CombatEventManager.AddSticker(sticker, card);

        Destroy(stickerView.gameObject);
    }
}