using CardZones;
using TMPro;
using UnityEngine;

namespace Views
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI countText;

        Deck deck;

        public void Bind(Deck d)
        {
            deck = d;

            d.OnCardAdded += OnChanged;
            d.OnCardRemoved += OnChanged;

            Refresh();
        }

        void OnDestroy()
        {
            if (deck == null) return;

            deck.OnCardAdded -= OnChanged;
            deck.OnCardRemoved -= OnChanged;
        }

        void OnChanged(Card _) => Refresh();

        void Refresh() => countText.text = $"({deck.Count})";
    }
}