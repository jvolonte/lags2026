using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
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

    [SerializeField] GameStateManager gameStateManager;

    CardView currentHoveredCard;
    float scaleDuration = 0.25f;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (gameStateManager.CurrentState != GameState.PlayerPlaceSticker)
            return;

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

        dragging.transform.DOScale(originalScale * stickerScaleMultiplier, scaleDuration);
        FaceCameraSmooth(dragging.transform);
        dragging.GetComponent<StickerView>().SetRenderOnTop(true);

        dragging.Dragging = true;
    }

    void Drag(Vector2 screenPos)
    {
        var ray = cam.ScreenPointToRay(screenPos);

        var planePos = cam.transform.position + cam.transform.forward * dragDistance;
        var plane = new Plane(-cam.transform.forward, planePos);

        CardView newHoveredCard = null;

        if (Physics.Raycast(ray, out var hit, 100f, cardLayer))
        {
            plane.SetNormalAndPosition(hit.normal, hit.point + hit.normal * 0.05f);

            newHoveredCard = hit.collider.GetComponent<CardView>()
                             ?? hit.collider.GetComponentInChildren<CardView>();

            if (newHoveredCard != null)
                RotateTowards(dragging.transform, newHoveredCard.transform.rotation);
            else
                FaceCameraSmooth(dragging.transform);
        }
        else
            FaceCameraSmooth(dragging.transform);

        if (newHoveredCard != currentHoveredCard)
        {
            if (currentHoveredCard != null && newHoveredCard == null)
                ResetDragScale();

            if (newHoveredCard != null)
                ApplyScaleForCard(newHoveredCard);

            currentHoveredCard = newHoveredCard;
        }

        if (plane.Raycast(ray, out var dist))
            dragging.transform.position = ray.GetPoint(dist);
    }

    void ApplyScaleForCard(CardView card)
    {
        var cardScale = card.transform.lossyScale;
        var scaleFactor = cardScale.x;
        var targetScale = originalScale * (scaleFactor * stickerScaleMultiplier);
        dragging.transform.DOScale(targetScale, scaleDuration);
    }

    void ResetDragScale() =>
        dragging.transform.DOScale(originalScale * stickerScaleMultiplier, scaleDuration);

    void EndDrag(Vector2 screenPos)
    {
        dragging.Dragging = false;

        var ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out var hit, 100f, cardLayer))
        {
            var card = hit.collider.GetComponent<CardView>()
                       ?? hit.collider.GetComponentInChildren<CardView>();

            if (card != null && card.canReceiveStickers)
            {
                ApplySticker(card, dragging);
                dragging = null;
                return;
            }
        }

        ReturnToOrigin(dragging);
        dragging = null;
        currentHoveredCard = null;
    }

    void ReturnToOrigin(StickerView sticker)
    {
        sticker.transform.DOScale(originalScale, scaleDuration);
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
        const float stickerZStep = -0.001f;
        var stickerCount = cardView.GetCard().Stickers.Count;
        stickerView.transform.localPosition = new Vector3(localPos.x, localPos.y, -0.02f + stickerCount * stickerZStep);
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