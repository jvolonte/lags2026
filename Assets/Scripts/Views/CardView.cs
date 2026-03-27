using System.Collections.Generic;
using Audio;
using Data;
using TMPro;
using UnityEngine;
using Utils;

namespace Views
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] CardTextureDatabase cardTextureDatabase;

        [SerializeField] HandCardView handCardView;
        [SerializeField] Renderer cardRenderer;
        [SerializeField] CardAnimations cardAnimation;
        [SerializeField] TextMeshProUGUI valueText;
        [SerializeField] TextMeshProUGUI valueTextRotated;
        [SerializeField] Transform stickerContainer;

        [SerializeField] float stickerScaleMultiplier = 0.5f;

        public Transform StickerContainer => stickerContainer;
        public CardAnimations CardAnimations => cardAnimation;

        public bool canReceiveStickers = false;

        Card card;

        readonly List<StickerView> stickerViews = new List<StickerView>();

        void OnEnable()
        {
            if (handCardView != null)
                handCardView.OnHoverChanged += OnHoverChange;
        }

        void OnDisable()
        {
            if (handCardView != null)
                handCardView.OnHoverChanged -= OnHoverChange;
        }

        public void SetCard(Card c)
        {
            SetValue(c.Value);
            SetVisual(c);
        }

        void SetValue(int value)
        {
            valueText.text = value.ToString();
            valueTextRotated.text = value.ToString();
        }

        void SetVisual(Card c)
        {
            SubscribeToChangedOnView(c);

            var texture = cardTextureDatabase.GetTexture(c.Suit, c.Value);
            cardRenderer.material.SetTexture("_MainTex", texture);

            RebuildStickers();
        }

        void SubscribeToChangedOnView(Card c)
        {
            if (card != null)
                card.OnStickerAdded -= RebuildStickers;

            card = c;

            if (card != null)
                card.OnStickerAdded += RebuildStickers;
        }

        void RebuildStickers()
        {
            stickerViews.Clear();
            stickerContainer.DeleteChildren();

            var baseZ = -0.02f;
            var zStep = -0.001f;

            for (var i = 0; i < card.Stickers.Count; i++)
            {
                var placement = card.Stickers[i];
                var view = Instantiate(placement.Data.prefab, stickerContainer, false);
                view.Bind(new StickerInstance { Logic = placement.Logic, Data = placement.Data });
                
                view.transform.localScale = Vector3.one * stickerScaleMultiplier;
                view.transform.localRotation = Quaternion.identity;

                var z = baseZ + (i * zStep);

                view.transform.localPosition = new Vector3(
                    placement.LocalPosition.x,
                    placement.LocalPosition.y,
                    z
                );

                view.DisableDragging();
                stickerViews.Add(view);
            }
        }

        void OnHoverChange(HandCardView view, bool hover) => cardAnimation.Highlight(hover);

        public void Burn(System.Action onEnd)
        {
            cardAnimation.Burn(onEnd);
            SfxManager.Play(SfxClipId.Burn);
        }

        public Card GetCard() => card;

        public void AllowStickers() => canReceiveStickers = true;

        public List<StickerView> GetStickers() => stickerViews;
    }
}