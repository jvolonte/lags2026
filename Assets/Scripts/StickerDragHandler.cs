using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;

public class StickerDragHandler : MonoBehaviour
{
    Camera cam;

    StickerView dragging;

    Vector3 originalPosition;
    Vector3 originalScale;
    Quaternion originalRotation;
    Transform originalParent;

    [SerializeField] float dragDistance = 2f;
    [SerializeField] LayerMask cardLayer;
    [SerializeField] float stickerScaleMultiplier = 0.5f;

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
            if (view == null || !view.CanDrag) return;

            StartDrag(view);
        }
    }

    void StartDrag(StickerView view)
    {
        dragging = view;

        originalParent = dragging.transform.parent;
        dragging.transform.SetParent(null, true);

        originalScale = dragging.transform.localScale;
        originalPosition = dragging.transform.position;
        originalRotation = dragging.transform.rotation;

        dragging.transform.DOScale(stickerScaleMultiplier, 0.15f);
        FaceCameraSmooth(dragging.transform);
        dragging.GetComponent<StickerView>().SetRenderOnTop(true);

        dragging.Dragging = true;
    }

    void Drag(Vector2 screenPos)
    {
        var ray = cam.ScreenPointToRay(screenPos);

        var planePos = cam.transform.position + cam.transform.forward * dragDistance;
        var plane = new Plane(-cam.transform.forward, planePos);

        if (Physics.Raycast(ray, out var hit, 100f, cardLayer))
        {
            plane.SetNormalAndPosition(hit.normal, hit.point + hit.normal * 0.05f);
            RotateTowards(dragging.transform, hit.transform.rotation);
        }
        else
            FaceCameraSmooth(dragging.transform);

        if (plane.Raycast(ray, out var dist))
            dragging.transform.position = ray.GetPoint(dist);
    }

    void EndDrag(Vector2 screenPos)
    {
        dragging.Dragging = false;

        var ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out var hit, 100f, cardLayer))
        {
            var card = hit.collider.GetComponent<CardView>()
                       ?? hit.collider.GetComponentInChildren<CardView>();

            //TODO: this should check if card is either in hand or in play.
            if (card != null)
            {
                ApplySticker(card, dragging);
                dragging = null;
                return;
            }
        }

        ReturnToOrigin(dragging);
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
        var container = cardView.StickerContainer;
        var localPos = container.InverseTransformPoint(stickerView.transform.position);

        var placement = new StickerPlacement
        {
            Data = stickerView.GetData(),
            Logic = stickerView.GetLogic(),
            LocalPosition = new Vector2(localPos.x, localPos.y)
        };

        stickerView.transform.SetParent(container, false);
        stickerView.transform.localPosition = new Vector3(localPos.x, localPos.y, -0.02f);
        stickerView.transform.localScale = Vector3.one * stickerScaleMultiplier;
        stickerView.transform.localRotation = Quaternion.identity;

        stickerView.SetRenderOnTop(false);
        stickerView.DisableDragging();

        CombatEventManager.AddSticker(placement, card);
    }

    void FaceCameraSmooth(Transform t)
    {
        var targetRot = Quaternion.LookRotation(Helpers.Camera.transform.forward, Vector3.up);
        RotateTowards(t, targetRot);
    }

    void RotateTowards(Transform t, Quaternion targetRot, float speed = 15f) =>
        t.rotation = Quaternion.Slerp(t.rotation, targetRot, Time.deltaTime * speed);
}