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
        [SerializeField] CardCombatView combatView;

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

            combatView.SetCard(card, true);
        }

        void HandleResolutionVisual(GameContext game, ResolutionContext resolution, ConflictOutcome conflictOutcome)
        {
            var card = combatView.GetCard();

            if (card == null) return;

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
                var card = combatView.GetCard();

                combatView.Hide(false, () => CombatEventManager.Discard(card));

                yield return transitionService.MoveAndSwap(
                    source: currentView.transform,
                    target: target,
                    proxyPrefab: currentView.gameObject, 
                    () => { });
            }
            else
            {
                //previewView.HideEvaluation();
                var done = false;
                var watchdog = 0f;

                currentView.Burn(() => done = true);
                combatView.Burn();

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

            //if (previewView != null)
            //    Destroy(previewView.gameObject);
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

                    //if (previewView != null)
                    //    Destroy(previewView.gameObject);

                    currentView = Instantiate(prefab, targetPosition, targetRotation, spawnPoint);
                    currentView.transform.localScale = targetScale;

                    currentView.SetCard(card, false);
                    currentView.AllowStickers();

                    StartCoroutine(InitializePreview(card));
                }
            );
        }

        IEnumerator InitializePreview(Card card)
        {
            yield return new WaitForSeconds(.5f);

            //previewView = Instantiate(prefab, currentView.transform.position, currentView.transform.rotation);
            //previewView.SetCard(card, isPlayer: true);
            //previewView.AllowStickers();

            //previewView.transform.localScale = Vector3.zero;
            //previewView.transform
            //           .DOScale(2f, 0.2f)
            //           .SetEase(Ease.OutBack);

            //yield return transitionService.Move(
            //    previewView.transform,
            //    previewAnchor.position,
            //    previewAnchor.rotation,
            //    1f
            //);

            //previewView.transform.SetParent(previewAnchor);

            yield return combatView.Show(card, false);

            CombatEventManager.PlayerEvaluationReady(combatView.EvaluationView, combatView);

            CombatEventManager.PlayerCardReachedPosition();
        }
    }
}