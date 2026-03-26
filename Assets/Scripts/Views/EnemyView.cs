using UnityEngine;

namespace Views
{
    public class EnemyView : MonoBehaviour
    {
        const int MAX_DUMMYCARDS = 8;

        [SerializeField] Animator animator;
        [SerializeField] GameObject cardDummy;
        [SerializeField] float handCardsAngle;

        bool initialized;
        Transform[] cards;
        int currentCardsAmount;

        void OnEnable()
        {
            Initialize();
        }

        void OnDestroy() => CombatEventManager.OnEnemyHealthChanged -= OnHealthChanged;

        void LateUpdate()
        {
            if (currentCardsAmount > 0)
            {
                var initialAngle = handCardsAngle * currentCardsAmount * 0.5f;

                for (var i = 0; i < cards.Length; i++)
                {
                    cards[i].localPosition = Vector3.forward * (0.01f * i) + Vector3.right * (0.03f * i);
                    cards[i].localRotation = Quaternion.Lerp(cards[i].localRotation,
                        Quaternion.Euler(0, 0, initialAngle - i * handCardsAngle),
                        Time.deltaTime * 7f);
                }
            }
        }

        void Initialize()
        {
            if (initialized) return;

            cards = new Transform[MAX_DUMMYCARDS];
            for (var i = 0; i < cards.Length; i++)
            {
                cards[i] = Instantiate(cardDummy, cardDummy.transform.parent).transform;
                cards[i].transform.GetChild(0).transform.localPosition = Vector3.up * 0.3f;
            }

            cardDummy.SetActive(false);
            SetCardsAmount(3);

            CombatEventManager.OnEnemyHealthChanged += OnHealthChanged;

            initialized = true;
        }

        public void SetCardsAmount(int amount)
        {
            for (var i = 0; i < cards.Length; i++)
                cards[i].gameObject.SetActive(i < amount);

            currentCardsAmount = amount;
        }

        void OnHealthChanged(int current, int max)
        {
            if (current == max) return;
            GetHurt();
        }

        public void GetHurt()
        {
            animator.Play("RecieveDamage", 0, 0f);
        }
    }
}