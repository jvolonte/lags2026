using CardZones;
using TMPro;
using UnityEngine;

namespace Views
{
    public class DiscardPileView : MonoBehaviour
    {
        [SerializeField] Transform cardAnchor;
        [SerializeField] CardView cardPrefab;
        [SerializeField] TextMeshProUGUI countText;

        DiscardPile pile;
        CardView currentTopView;

        public void Bind(DiscardPile d)
        {
            pile = d;

            d.OnCardAdded += OnCardAdded;
            d.OnCardRemoved += OnCardRemoved;
            d.OnEmptied += OnEmptied;

            Refresh();
        }

        void OnEmptied()
        {
            if (currentTopView != null)
            {
                Destroy(currentTopView.gameObject);
                currentTopView = null;
            }

            RefreshCount();
        }

        void OnDestroy()
        {
            if (pile == null) return;

            pile.OnCardAdded -= OnCardAdded;
            pile.OnCardRemoved -= OnCardRemoved;
            pile.OnEmptied -= OnEmptied;
        }

        void OnCardAdded(Card card)
        {
            RefreshTopCard(card);
            RefreshCount();
        }

        void OnCardRemoved(Card _) => Refresh();

        void Refresh()
        {
            RefreshCount();

            var top = pile.Peek();
            RefreshTopCard(top);
        }

        void RefreshCount() => countText.text = $"({pile.Count})";

        void RefreshTopCard(Card card)
        {
            if (currentTopView != null)
                Destroy(currentTopView.gameObject);

            if (card == null)
                return;

            currentTopView = Instantiate(cardPrefab, cardAnchor.position, cardAnchor.rotation);
            currentTopView.SetCard(card, false);
        }
    }
}