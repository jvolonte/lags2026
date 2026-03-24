using Data;
using TMPro;
using UnityEngine;

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

        public EvaluationView evaluationView;
        public Transform StickerContainer => stickerContainer;
        public CardAnimations CardAnimations => cardAnimation;

        Card card;

        void OnEnable()
        {
            if (handCardView != null)
            {
                handCardView.OnHoverChanged += OnHoverChange;
            }
        }

        void OnDisable()
        {
            if (handCardView != null)
            {
                handCardView.OnHoverChanged -= OnHoverChange;
            }
        }

        public void SetCard(Card card, bool showEvaluation = true)
        {
            SetValue(card.Value);
            SetVisual(card);

            evaluationView.gameObject.SetActive(showEvaluation);
        }

        void SetValue(int value)
        {
            valueText.text = value.ToString();
            valueTextRotated.text = value.ToString();
            evaluationView.SetValue(value);
        }

        void SetVisual(Card c)
        {
            card = c;
            var texture = cardTextureDatabase.GetTexture(c.Suit, c.Value);
            cardRenderer.material.SetTexture("_MainTex", texture);
            
            RebuildStickers();
        }

        void RebuildStickers()
        {
            foreach (var placement in card.Stickers)
            {
                var view = Instantiate(placement.Data.prefab, stickerContainer, false);

                view.Bind(new StickerInstance
                {
                    Logic = placement.Logic,
                    Data = placement.Data
                });

                view.transform.localScale = Vector3.one * stickerScaleMultiplier;
                view.transform.localRotation = Quaternion.identity;
                view.transform.localPosition = new Vector3(
                    placement.LocalPosition.x,
                    placement.LocalPosition.y,
                    -0.02f
                );

                view.DisableDragging();
            }
        }

        void OnHoverChange(HandCardView view, bool hover)
        {
            cardAnimation.Highlight(hover);
        }

        public Card GetCard() => card;
    }
}