using DG.Tweening;
using Services;
using System.Collections;
using UnityEngine;
using Views;

public class CardCombatView : MonoBehaviour
{
    [SerializeField] CardView prefab;
    [SerializeField] Transform root;
    [SerializeField] Transform cardSpawnParent;
    [SerializeField] EvaluationView evaluationView;
    [SerializeField] float transitionDuration;
    [SerializeField] bool appearFromLeft;

    [Header("Scene References")]
    [SerializeField] ViewTransitionService transitionService;
    [SerializeField] DiscardPileView discardPileView;

    CardView previewView;
    Coroutine transition;

    private void Awake()
    {
        root.gameObject.SetActive(false);
    }

    public void Show (Card card, bool isEnemy, System.Action onEnd = null) =>
        StartCoroutine(Transition(ShowTransition(card, isEnemy, transitionDuration), onEnd));

    public void Hide (bool isEnemy, System.Action onEnd = null) =>
        StartCoroutine(Transition(HideTransition(isEnemy, transitionDuration), onEnd));

    public void Burn (System.Action onEnd = null) =>
        StartCoroutine(Transition(BurnTransition(), onEnd));

    public void Discard (System.Action onEnd = null) =>
        StartCoroutine(Transition(DiscardTransition(), onEnd));


    IEnumerator Transition(IEnumerator transition, System.Action onEnd = null)
    {
        if (this.transition != null) StopCoroutine(transition);

        this.transition = StartCoroutine(transition);
        yield return transition;
        this.transition = null;
        onEnd?.Invoke();
    }

    IEnumerator ShowTransition (Card card, bool isEnemy, float duration)
    {
        root.gameObject.SetActive(false);

        previewView = Instantiate(prefab, cardSpawnParent.position, cardSpawnParent.rotation);
        previewView.transform.localScale = Vector3.one;
        previewView.SetCard(card);

        root.transform.localPosition = Vector3.right * 5f;
        root.transform.localPosition *= appearFromLeft ? -1 : 1;
        root.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);

        root.gameObject.SetActive(true);

        yield return DOTween.Sequence()
                   .Join(root.DOMove(Vector3.zero, duration))
                   .Join(root.DORotateQuaternion(Quaternion.identity, duration * 1.1f))
                   .SetEase(Ease.OutCubic);

        if (isEnemy)
            CombatEventManager.EnemyEvaluationReady(evaluationView);
        else
            CombatEventManager.PlayerEvaluationReady(evaluationView);

    }

    IEnumerator HideTransition (bool isEnemy, float duration)
    {
        Vector3 goalPos = Vector3.right * 5f;
        goalPos *= appearFromLeft ? -1 : 1;

        yield return DOTween.Sequence()
                   .Join(root.DOMove(goalPos, duration))
                   .Join(root.DORotateQuaternion(Quaternion.identity, duration))
                   .SetEase(Ease.OutCubic);

        root.gameObject.SetActive(false);

        if (previewView) 
            Destroy(previewView.gameObject);
    }

    IEnumerator BurnTransition ()
    {
        bool done = false;

        previewView.CardAnimations.Burn(() => done = true);

        while (!done) yield return null;

        root.gameObject.SetActive(false);
    }

    IEnumerator DiscardTransition ()
    {
        var target = discardPileView.GetAnchor();

        if (previewView)
            Destroy(previewView.gameObject);

        yield return transitionService.MoveAndSwap(
            source: previewView.transform,
            target: target,
            proxyPrefab: previewView.gameObject, () => { });

        root.gameObject.SetActive(false);
    }
}
