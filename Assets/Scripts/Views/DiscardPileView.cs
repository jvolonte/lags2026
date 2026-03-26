using CardZones;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Views
{
    public class DiscardPileView : MonoBehaviour
    {
        const int MAX_DUMMYCARDS = 8;

        [SerializeField] GameObject cardDummy;

        Transform[] dummyCards;
        Quaternion baseRot;
        bool initialized;

        [SerializeField] Transform cardAnchor;
        [SerializeField] CardView cardPrefab;
        [SerializeField] TextMeshProUGUI countText;

        DiscardPile pile;
        CardView currentTopView;

        public Transform GetAnchor() => cardAnchor;

        void Awake() => Initialize();

        void Initialize()
        {
            if (initialized) return;

            dummyCards = new Transform[MAX_DUMMYCARDS];

            for (var i = 0; i < dummyCards.Length; i++)
                dummyCards[i] = Instantiate(cardDummy, cardAnchor).transform;

            cardDummy.SetActive(false);
            baseRot = cardDummy.transform.localRotation;

            initialized = true;
        }

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
            Refresh();
        }

        void OnCardRemoved(Card _) =>
            Refresh();

        void Refresh()
        {
            RefreshCount();

            var top = pile.Peek();
            RefreshTopCard(top);
            UpdateDummyCards();
        }

        void RefreshCount() => countText.text = $"({pile.Count})";

        void RefreshTopCard(Card card)
        {
            if (currentTopView != null)
                Destroy(currentTopView.gameObject);

            if (card == null)
                return;

            currentTopView = Instantiate(cardPrefab, cardAnchor);
            var stackHeight = Mathf.Clamp(pile.Count - 1, 0, MAX_DUMMYCARDS);

            var topZ = 0.002f * (stackHeight + 1);
            currentTopView.transform.localPosition = new Vector3(0f, 0.02f * stackHeight, topZ);

            currentTopView.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-2f, 2f));

            currentTopView.SetCard(card, false);
        }

        void UpdateDummyCards()
        {
            var total = pile.Count;
            var dummyAmount = Mathf.Clamp(total - 1, 0, MAX_DUMMYCARDS);
            var faceUpRotation = Quaternion.Euler(180f, 0f, 180f);

            for (var i = 0; i < dummyCards.Length; i++)
            {
                var obj = dummyCards[i].gameObject;

                if (i < dummyAmount)
                {
                    obj.SetActive(true);

                    var stackOffset = 0.002f * i;

                    dummyCards[i].localPosition = new Vector3(Random.Range(-0.01f, 0.01f), 0.02f * i, stackOffset);

                    dummyCards[i].localPosition =
                        new Vector3(Random.Range(-0.01f, 0.01f), 0.02f * i, Random.Range(-0.01f, 0.01f));

                    dummyCards[i].localRotation =
                        faceUpRotation * Quaternion.AngleAxis(Random.Range(-3f, 3f), Vector3.forward);
                }
                else
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}