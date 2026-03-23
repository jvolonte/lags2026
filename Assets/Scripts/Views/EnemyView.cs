using UnityEngine;

public class EnemyView : MonoBehaviour
{
    private const int MAX_DUMMYCARDS = 8;

    [SerializeField] Animator animator;
    [SerializeField] GameObject cardDummy;
    [SerializeField] float handCardsAngle;

    bool initialized;
    Transform[] cards;
    int currentCardsAmount;

    private void OnEnable()
    {   
        Initialize();
    }
    private void OnDestroy()
    {
        CombatEventManager.OnEnemyHealthChanged -= OnHealthChanged;
    }
    private void LateUpdate()
    {
        if (currentCardsAmount > 0)
        {
            float initialAngle = handCardsAngle * currentCardsAmount * 0.5f;

            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].localPosition = Vector3.forward * 0.01f * i + Vector3.right * 0.03f * i;
                cards[i].localRotation = Quaternion.Lerp(cards[i].localRotation, 
                    Quaternion.Euler(0, 0, initialAngle - i * handCardsAngle), 
                    Time.deltaTime * 7f);
            }
        }
    }
    void Initialize ()
    {
        if (initialized) return;

        cards = new Transform[MAX_DUMMYCARDS];
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = Instantiate(cardDummy, cardDummy.transform.parent).transform;
        }
        cardDummy.SetActive(false);
        SetCardsAmount(3);

        CombatEventManager.OnEnemyHealthChanged += OnHealthChanged;
    }
    public void SetCardsAmount (int amount)
    {
        for (int i = 0;i < cards.Length;i++)
        {
            cards[i].gameObject.SetActive(i < amount);
        }
        currentCardsAmount = amount;
    }
    private void OnHealthChanged (int current, int max)
    {
        if (current == max) return;
        GetHurt();
    }
    public void GetHurt ()
    {
        animator.Play("RecieveDamage", 0, 0f);
    }
}
