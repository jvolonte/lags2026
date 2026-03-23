using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;

public class StickerDragHandler : MonoBehaviour
{
    Camera cam;

    StickerView dragging;
    Vector3 offset;

    Vector3 originalPosition;
    Vector3 originalScale;
    Quaternion originalRotation;
    Transform originalParent;

    [SerializeField] float dragDistance = 2f;

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

            originalScale = dragging.transform.localScale;
            originalPosition = dragging.transform.position;
            originalRotation = dragging.transform.rotation;
            originalParent = dragging.transform.parent;
            
            offset = dragging.transform.position - hit.point;

            dragging.transform.DOScale(0.4f, 0.15f);
            dragging.transform.SetParent(null);
            FaceCameraSmooth(dragging.transform);
            dragging.GetComponent<StickerView>().SetRenderOnTop(true);
        }
    }

    void Drag(Vector2 screenPos)
    {
        var ray = cam.ScreenPointToRay(screenPos);

        var planePos = cam.transform.position + cam.transform.forward * dragDistance;
        var plane = new Plane(-cam.transform.forward, planePos);

        if (plane.Raycast(ray, out var dist))
        {
            var point = ray.GetPoint(dist);
            dragging.transform.position = point + offset;
            FaceCameraSmooth(dragging.transform);
        }
    }

    void EndDrag(Vector2 screenPos)
    {
        var ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out var hit))
        {
            var card = hit.collider.GetComponentInParent<CardView>();

            if (card != null)
            {
                ApplySticker(card, dragging);
                dragging = null;
                return;
            }
        }

        ReturnToOrigin(dragging);
        // dragging.GetComponent<StickerView>().SetRenderOnTop(false);
        dragging = null;
    }

    void ReturnToOrigin(StickerView sticker)
    {
        sticker.transform.DOScale(originalScale, 0.2f);
        sticker.transform.SetParent(originalParent);
        sticker.transform.DOMove(originalPosition, 0.25f).SetEase(Ease.OutQuad);
        sticker.transform.DORotateQuaternion(originalRotation, 0.25f);
    }

    void ApplySticker(CardView cardView, StickerView stickerView)
    {
        var card = cardView.GetCard();
        var sticker = stickerView.GetLogic();

        card.Stickers.Add(sticker);

        CombatEventManager.AddSticker(sticker, card);

        Destroy(stickerView.gameObject);
    }

    void FaceCameraSmooth(Transform t)
    {
        var targetRot = Quaternion.LookRotation(-Helpers.Camera.transform.forward, Vector3.up);
        t.rotation = Quaternion.Slerp(t.rotation, targetRot, Time.deltaTime * 15f);
    }
}