using UnityEngine;
using Views;

public class StickerShadow : MonoBehaviour
{
    [SerializeField] Transform stickerShadow;
    [SerializeField] Vector3 shadowOffset;
    
    StickerView stickerView;
    MeshRenderer shadowMesh;

    CardView[] worldCards;
    CardAnimations closestCard;
    Plane shadowPlane;
    Ray shadowRay;

    private void OnDestroy()
    {
        closestCard?.shadowReciever.SetActive(false);
    }
    private void OnDisable()
    {
        closestCard?.shadowReciever.SetActive(false);
    }
    private void OnEnable()
    {
        stickerView = GetComponent<StickerView>();
        shadowMesh = stickerShadow.GetComponent<MeshRenderer>();
        shadowMesh.material.SetTexture("_MainTex", 
            stickerView.MeshRenderer.material.GetTexture("_MainTex"));

        GetWorldCards();
    }
    private void LateUpdate()
    {
        if (worldCards == null || worldCards.Length == 0) return;

        float closest = float.MaxValue;
        CardView newClosest = null;

        for (int i = 0; i < worldCards.Length; i++)
        {
            if (worldCards[i] == null) continue;

            float d = (worldCards[i].transform.position - transform.position).sqrMagnitude;
            if (d < closest)
            {
                closest = d;
                newClosest = worldCards[i];
            }
        }

        if (newClosest != closestCard)
        {
            closestCard?.shadowReciever.SetActive(false);
            newClosest?.CardAnimations.shadowReciever.SetActive(true);
            closestCard = newClosest.CardAnimations;
        }

        if (closestCard)
        {
            shadowPlane.SetNormalAndPosition(
                closestCard.transform.forward, 
                closestCard.shadowReciever.transform.position);

            shadowRay.origin = transform.position;
            shadowRay.direction = -closestCard.transform.forward;
            float distance = 0;
            shadowPlane.Raycast(shadowRay, out distance);

            stickerShadow.position = shadowRay.GetPoint(distance);
            stickerShadow.position += closestCard.transform.TransformDirection(shadowOffset);
            stickerShadow.rotation = Quaternion.LookRotation(-closestCard.transform.forward, transform.up);
        }
    }
    public void GetWorldCards()
    {
        worldCards = FindObjectsByType<CardView>(FindObjectsSortMode.InstanceID);
    }
}
