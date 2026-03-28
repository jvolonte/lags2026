using UnityEngine;
using Views;

namespace Animation
{
    public class StickerShadow : MonoBehaviour
    {
        [SerializeField] Transform stickerShadow;
        [SerializeField] Vector3 shadowOffset;

        public Transform Shadow => stickerShadow;

        StickerView stickerView;
        MeshRenderer shadowMesh;

        CardView[] worldCards;
        CardAnimations closestCard;
        Plane shadowPlane;
        Ray shadowRay;

        void OnDestroy()
        {
            if (closestCard != null && closestCard.shadowReciever != null)
                closestCard?.shadowReciever.SetActive(false);
        }

        void OnDisable()
        {
            if (closestCard != null && closestCard.shadowReciever != null)
                closestCard?.shadowReciever.SetActive(false);
        }

        void OnEnable()
        {
            stickerView = GetComponent<StickerView>();
            shadowMesh = stickerShadow.GetComponent<MeshRenderer>();

            UpdateShadowTexture();

            GetWorldCards();
        }

        void LateUpdate()
        {
            if (stickerShadow.gameObject.activeSelf != stickerView.Dragging)
                stickerShadow.gameObject.SetActive(stickerView.Dragging);

            if (!stickerView.Dragging)
                return;

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
                stickerShadow.rotation = Quaternion.LookRotation(closestCard.transform.forward, transform.up);
            }
        }

        public void UpdateShadowTexture()
        {
            if (stickerView == null) return;

            Texture texSticker = stickerView.MeshRenderer.material.GetTexture("_MainTex");
            shadowMesh.material.SetTexture("_ShadowTex", texSticker);
            stickerShadow.localScale = stickerView.MeshRenderer.transform.localScale;
        }

        public void GetWorldCards()
        {
            worldCards = FindObjectsByType<CardView>(FindObjectsSortMode.InstanceID);
        }
    }
}