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

        [SerializeField] Transform playerEvaluationPos;
        [SerializeField] Transform enemyEvaluationPos;

        [SerializeField] float stickerScaleMultiplier = 0.5f;

        public EvaluationView evaluationView;
        public Transform StickerContainer => stickerContainer;
        public CardAnimations CardAnimations => cardAnimation;

        public bool canReceiveStickers = false;

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

        public void SetCard(Card c, bool showEvaluation = true, bool isPlayer = false)
        {
            SetValue(c.Value);
            SetVisual(c);

            evaluationView.gameObject.SetActive(showEvaluation);
            evaluationView.transform.position = (isPlayer ? playerEvaluationPos : enemyEvaluationPos).position;
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
            var baseZ = -0.02f;
            var zStep = -0.001f;

            for (var i = 0; i < card.Stickers.Count; i++)
            {
                var placement = card.Stickers[i];

                var view = Instantiate(placement.Data.prefab, stickerContainer, false);

                view.Bind(new StickerInstance
                {
                    Logic = placement.Logic,
                    Data = placement.Data
                });

                view.transform.localScale = Vector3.one * stickerScaleMultiplier;
                view.transform.localRotation = Quaternion.identity;

                var z = baseZ + (i * zStep);

                view.transform.localPosition = new Vector3(
                    placement.LocalPosition.x,
                    placement.LocalPosition.y,
                    z
                );

                view.DisableDragging();
            }
        }

        void OnHoverChange(HandCardView view, bool hover)
        {
            cardAnimation.Highlight(hover);
        }

        public void Burn(System.Action onEnd)
        {
            cardAnimation.Burn(onEnd);
        }

        public Card GetCard() => card;

        public void AllowStickers() => canReceiveStickers = true;

        public void ShowEvaluation() => evaluationView.gameObject.SetActive(true);
        
        public void HideEvaluation() => evaluationView.gameObject.SetActive(false);
    }
}