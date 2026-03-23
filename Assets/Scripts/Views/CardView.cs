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

        public EvaluationView evaluationView;

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
        }

        void OnHoverChange(HandCardView view, bool hover)
        {
            cardAnimation.Highlight(hover);
        }

        public Card GetCard() => card;
    }
}