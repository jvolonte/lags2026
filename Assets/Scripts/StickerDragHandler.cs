using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using Views;

public class StickerDragHandler : MonoBehaviour
{
    Camera cam;

    Vector3 originalPosition;
    Vector3 originalScale;
    Quaternion originalRotation;
    Transform originalParent;

    [SerializeField] float dragDistance = 2f;
    [SerializeField] LayerMask cardLayer;
    [SerializeField] float stickerScaleMultiplier = 0.5f;

    [SerializeField] GameStateManager gameStateManager;

    CardView currentHoveredCard;
    StickerView hoverSticker;
    StickerView draggingSticker;
    float scaleDuration = 0.25f;
    Ray ray;

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

        StickerView sticker = null;
        if (draggingSticker == null)
        {
            ray = cam.ScreenPointToRay(mouse.position.ReadValue());
            
            if (Physics.Raycast(ray, out var hit))
            {
                var view = hit.collider.GetComponentInParent<StickerView>();
                if (view != null && view.CanDrag)
                    sticker = view;
            }  
        }

        if (sticker != hoverSticker)
        {
            Debug.Log("New: " + sticker);
            Debug.Log("Old: " + hoverSticker);
            hoverSticker?.Highlight(false);
            sticker?.Highlight(true);
        }
        hoverSticker = sticker;

        if (mouse.leftButton.wasPressedThisFrame && draggingSticker == null)
            TryStartDrag(mouse.position.ReadValue());

        if (mouse.leftButton.isPressed && draggingSticker != null)
            Drag(mouse.position.ReadValue());

        if (mouse.leftButton.wasReleasedThisFrame && draggingSticker != null)
            EndDrag(mouse.position.ReadValue());
    }

    void TryStartDrag(Vector2 screenPos)
    {
        if (hoverSticker == null) return;

        StartDrag(hoverSticker);
    }

    void StartDrag(StickerView view)
    {
        draggingSticker = view;

        originalParent = draggingSticker.transform.parent;
        draggingSticker.transform.SetParent(null, true);

        originalScale = draggingSticker.transform.localScale;
        originalPosition = draggingSticker.transform.position;
        originalRotation = draggingSticker.transform.rotation;

        draggingSticker.transform.DOScale(originalScale * stickerScaleMultiplier, scaleDuration).SetEase(Ease.OutBack);
        FaceCameraSmooth(draggingSticker.transform);
        draggingSticker.GetComponent<StickerView>().SetRenderOnTop(true);

        draggingSticker.Dragging = true;
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

            if (newHoveredCard != null && newHoveredCard.canReceiveStickers)
                RotateTowards(draggingSticker.transform, newHoveredCard.transform.rotation);
            else
                FaceCameraSmooth(draggingSticker.transform);
        }
        else
            FaceCameraSmooth(draggingSticker.transform);

        if (newHoveredCard != currentHoveredCard)
        {
            if (currentHoveredCard != null && newHoveredCard == null)
                ResetDragScale();

            if (newHoveredCard != null)
                ApplyScaleForCard(newHoveredCard);

            currentHoveredCard = newHoveredCard;
        }

        if (plane.Raycast(ray, out var dist))
            draggingSticker.transform.position = ray.GetPoint(dist);
    }

    void ApplyScaleForCard(CardView card)
    {
        var cardScale = card.transform.lossyScale;
        var scaleFactor = cardScale.x;
        var targetScale = originalScale * (scaleFactor * stickerScaleMultiplier);
        draggingSticker.transform.DOScale(targetScale, scaleDuration).SetEase(Ease.OutBack)
            ;
    }

    void ResetDragScale() =>
        draggingSticker.transform.DOScale(originalScale * stickerScaleMultiplier, scaleDuration).SetEase(Ease.OutBack);

    void EndDrag(Vector2 screenPos)
    {
        draggingSticker.Dragging = false;

        var ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out var hit, 100f, cardLayer))
        {
            var card = hit.collider.GetComponent<CardView>()
                       ?? hit.collider.GetComponentInChildren<CardView>();

            if (card != null && card.canReceiveStickers)
            {
                ApplySticker(card, draggingSticker);
                draggingSticker = null;
                return;
            }
        }

        ReturnToOrigin(draggingSticker);
        draggingSticker = null;
        currentHoveredCard = null;
    }

    void ReturnToOrigin(StickerView sticker)
    {
        sticker.transform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutBack);
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