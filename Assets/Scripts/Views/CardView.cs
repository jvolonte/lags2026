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


        private void OnEnable()
        {
            if (handCardView != null)
            {
                handCardView.OnHoverChanged += OnHoverChange;
            }
        }
        private void OnDisable()
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

        void SetVisual(Card card)
        {
            var texture = cardTextureDatabase.GetTexture(card.Suit, card.Value);
            cardRenderer.material.SetTexture("_MainTex", texture);
        }

        void OnHoverChange(HandCardView card, bool hover)
        {
            cardAnimation.Highlight(hover);
        }
    }
}