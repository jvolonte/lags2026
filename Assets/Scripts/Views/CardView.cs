using TMPro;
using UnityEngine;

namespace Views
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] Renderer cardRenderer;
        [SerializeField] TextMeshProUGUI valueText;
        [SerializeField] TextMeshProUGUI valueTextRotated; 
        public EvaluationView evaluationView;

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
            cardRenderer.material.color = GetColor(card.Suit);
        }

        Color GetColor(Suit suit) =>
            suit switch
            {
                Suit.Cups => Color.red,
                Suit.Swords => Color.gray,
                Suit.Clubs => Color.green,
                Suit.Golds => Color.yellow,
                _ => Color.white
            };
    }
}