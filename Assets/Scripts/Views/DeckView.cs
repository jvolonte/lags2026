using CardZones;
using TMPro;
using UnityEngine;

namespace Views
{
    public class DeckView : MonoBehaviour
    {
        const int MAX_DUMMYCARDS = 8;

        [SerializeField] GameObject cardDummy;
        [SerializeField] TextMeshProUGUI countText;
        [SerializeField] Transform deckUI;

        bool initialized;
        Transform[] cards;
        Quaternion baseRot;
        Deck deck;

        void Awake()
        {
            Initialize();
        }

        public Transform GetDrawAnchor() => deckUI;

        void Initialize()
        {
            if (initialized) return;

            cards = new Transform[MAX_DUMMYCARDS];
            for (var i = 0; i < cards.Length; i++)
                cards[i] = Instantiate(cardDummy, cardDummy.transform.parent).transform;

            cardDummy.SetActive(false);

            baseRot = cardDummy.transform.localRotation;

            initialized = true;
        }

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

        void Refresh()
        {
            countText.text = $"{deck.Count}";
            SetCardsAmount(deck.Count);
        }

        void SetCardsAmount(int amount)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].gameObject.SetActive(i < amount);
                cards[i].localPosition = Vector3.up * (0.03f * i);
                cards[i].localRotation = baseRot * Quaternion.AngleAxis(Random.Range(-3, 3), Vector3.forward);
                if (i < amount)
                {
                    var deckUIPosition = deckUI.localPosition;
                    deckUIPosition.y = cards[i].localPosition.y + 0.1f;
                    deckUI.localPosition = deckUIPosition;
                }
            }
        }
    }
}