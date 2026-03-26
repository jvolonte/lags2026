using System.Collections;
using DG.Tweening;
using Services;
using UnityEngine;
using Views;

namespace Presenters
{
    public class PlayerCardPresenter : MonoBehaviour
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] CardView prefab;
        [SerializeField] ViewTransitionService transitionService;
        [SerializeField] DiscardPileView discardPileView;

        [SerializeField] Transform previewAnchor;

        CardView previewView;
        CardView currentView;

        void Awake()
        {
            CombatEventManager.OnPlayCard += HandlePlayerCardPlayed;
            CombatEventManager.OnResolveCardsVisual += HandleResolutionVisual;
        }

        void OnDestroy()
        {
            CombatEventManager.OnPlayCard -= HandlePlayerCardPlayed;
            CombatEventManager.OnResolveCardsVisual -= HandleResolutionVisual;
        }

        public void UpdateCards(Card card)
        {
            if (currentView != null)
                currentView.SetCard(card, showEvaluation: false, isPlayer: true);

            if (previewView != null)
                previewView.SetCard(card, showEvaluation: true, isPlayer: true);
        }

        void HandleResolutionVisual(GameContext game, ResolutionContext resolution, ConflictOutcome conflictOutcome)
        {
            if (previewView == null)
                return;

            var card = previewView.GetCard();
            var fate = GetFinalFate(conflictOutcome, resolution.Get(card));
            StartCoroutine(AnimateResolution(fate));
        }

        CardFate GetFinalFate(ConflictOutcome round, CardOutcome effect)
        {
            return effect.Destroy || effect.WillBeLost
                ? CardFate.Destroy
                : round switch
                {
                    ConflictOutcome.PlayerWin => CardFate.Discard,
                    ConflictOutcome.EnemyWin => CardFate.Destroy,
                    ConflictOutcome.Tie => CardFate.Discard,
                    _ => CardFate.Destroy
                };
        }

        IEnumerator AnimateResolution(CardFate fate)
        {
            if (fate is CardFate.Discard)
            {
                var target = discardPileView.GetAnchor();

                yield return transitionService.MoveAndSwap(
                    source: previewView.transform,
                    target: target,
                    proxyPrefab: previewView.gameObject,
                    onArrive: () => CombatEventManager.Discard(previewView.GetCard()));
            }
            else
            {
                previewView.HideEvaluation();
                var done = false;
                var watchdog = 0f;

                currentView.Burn(() => done = true);
                previewView.Burn(() => { });

                while (!done && watchdog < 3f)
                {
                    watchdog += Time.deltaTime;
                    yield return null;
                }
            }

            HandleClearTable();
        }

        void HandleClearTable()
        {
            if (currentView != null)
                Destroy(currentView.gameObject);

            if (previewView != null)
                Destroy(previewView.gameObject);
        }

        void HandlePlayerCardPlayed(CardView source) =>
            StartCoroutine(PlayCardTransition(source.GetCard(), source));

        IEnumerator PlayCardTransition(Card card, CardView sourceView)
        {
            var targetPosition = spawnPoint.position;
            var targetRotation = spawnPoint.rotation;
            var targetScale = spawnPoint.localScale;

            yield return transitionService.MoveAndSwap(
                source: sourceView.transform,
                target: spawnPoint,
                proxyPrefab: sourceView.gameObject,
                onArrive: () =>
                {
                    if (currentView != null)
                        Destroy(currentView.gameObject);

                    if (previewView != null)
                        Destroy(previewView.gameObject);

                    currentView = Instantiate(prefab, targetPosition, targetRotation, spawnPoint);
                    currentView.transform.localScale = targetScale;

                    currentView.SetCard(card, false);
                    currentView.AllowStickers();

                    CombatEventManager.PlayerEvaluationReady(currentView.evaluationView);

                    StartCoroutine(InitializePreview(card));
                }
            );
        }

        IEnumerator InitializePreview(Card card)
        {
            yield return new WaitForSeconds(.5f);

            previewView = Instantiate(prefab, currentView.transform.position, currentView.transform.rotation);
            previewView.SetCard(card, isPlayer: true);
            previewView.AllowStickers();

            previewView.transform.localScale = Vector3.zero;
            previewView.transform
                       .DOScale(2f, 0.2f)
                       .SetEase(Ease.OutBack);

            yield return transitionService.Move(
                previewView.transform,
                previewAnchor.position,
                previewAnchor.rotation,
                1f
            );

            previewView.transform.SetParent(previewAnchor);
            CombatEventManager.PlayerCardReachedPosition();
        }
    }
}