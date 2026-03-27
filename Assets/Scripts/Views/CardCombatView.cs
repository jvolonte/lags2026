using DG.Tweening;
using Services;
using System.Collections;
using UnityEngine;
using Views;

public class CardCombatView : MonoBehaviour
{
    private const string OVERLAY_LAYER = "Overlay";

    [SerializeField] CardView prefab;
    [SerializeField] Transform root;
    [SerializeField] Transform cardSpawnParent;
    [SerializeField] EvaluationView evaluationView;
    [SerializeField] float transitionDuration;
    [SerializeField] bool appearFromLeft;

    [Header("Scene References")]
    [SerializeField] ViewTransitionService transitionService;
    [SerializeField] DiscardPileView discardPileView;

    public EvaluationView EvaluationView => evaluationView;

    CardView previewView;
    Coroutine transition;

    private void Awake()
    {
        root.gameObject.SetActive(false);
    }

    public Coroutine Show (Card card, bool isEnemy, System.Action onEnd = null) =>
        StartCoroutine(Transition(ShowTransition(card, isEnemy, transitionDuration), onEnd));

    public Coroutine Hide (bool isEnemy, System.Action onEnd = null) =>
        StartCoroutine(Transition(HideTransition(isEnemy, transitionDuration), onEnd));

    public Coroutine Burn (System.Action onEnd = null) =>
        StartCoroutine(Transition(BurnTransition(), onEnd));

    public Coroutine Discard (System.Action onEnd = null) =>
        StartCoroutine(Transition(DiscardTransition(), onEnd));

    public void SetValue(int value) =>
        evaluationView.SetValue(value);
    public void SetCard (Card card, bool allowStickers)
    {
        if (previewView)
            Destroy(previewView.gameObject);

        previewView = Instantiate(prefab, cardSpawnParent);
        previewView.transform.localPosition = Vector3.zero;
        previewView.transform.localRotation = Quaternion.identity;
        previewView.transform.localScale = Vector3.one;
        previewView.SetCard(card);
        if (allowStickers) previewView.AllowStickers();

        previewView.gameObject.ReplaceLayerRecursively("Default", OVERLAY_LAYER);
    }
    public Card GetCard ()
    {
        if (previewView)
            return previewView.GetCard();

        return null;
    }

    IEnumerator Transition(IEnumerator transition, System.Action onEnd = null)
    {
        if (this.transition != null) StopCoroutine(transition);

        this.transition = StartCoroutine(transition);
        yield return this.transition;
        this.transition = null;
        onEnd?.Invoke();
    }

    IEnumerator ShowTransition (Card card, bool isEnemy, float duration)
    {
        root.gameObject.SetActive(false);

        SetCard(card, allowStickers: !isEnemy);  
        SetValue(card.Value);

        root.transform.localPosition = Vector3.right * 5f;
        root.transform.localPosition *= appearFromLeft ? -1 : 1;
        root.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);

        root.gameObject.SetActive(true);

        yield return DOTween.Sequence()
                   .Join(root.DOLocalMove(Vector3.zero, duration))
                   .Join(root.DOLocalRotateQuaternion(Quaternion.identity, duration * 1.1f))
                   .SetEase(Ease.OutCubic)
                   .WaitForCompletion();
    }

    IEnumerator HideTransition (bool isEnemy, float duration)
    {
        Vector3 goalPos = Vector3.right * 5f;
        goalPos *= appearFromLeft ? -1 : 1;

        yield return DOTween.Sequence()
                   .Join(root.DOLocalMove(goalPos, duration))
                   .Join(root.DOLocalRotateQuaternion(Quaternion.identity, duration))
                   .SetEase(Ease.Linear)
                   .WaitForCompletion();

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
