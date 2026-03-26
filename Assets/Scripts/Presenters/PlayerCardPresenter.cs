using System.Collections;
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

        CardView currentView;

        void Awake()
        {
            CombatEventManager.OnPlayCard += HandlePlayerCardPlayed;
            CombatEventManager.OnResolveCardsVisual += HandleResolutionVisual;
        }

        void HandleResolutionVisual(GameContext game, ResolutionContext resolution, ConflictOutcome conflictOutcome)
        {
            if (currentView == null)
                return;

            var card = currentView.GetCard();
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
                    source: currentView.transform,
                    target: target,
                    proxyPrefab: currentView.gameObject,
                    onArrive: () => CombatEventManager.Discard(currentView.GetCard()));
            }
            else
            {
                //TODO: trigger card lost effect!
                Debug.Log("Disappear player card effect");
            }

            HandleClearTable();
        }

        void HandleClearTable()
        {
            if (currentView != null)
                Destroy(currentView.gameObject);
        }

        void OnDestroy()
        {
            CombatEventManager.OnPlayCard -= HandlePlayerCardPlayed;
            CombatEventManager.OnResolveCardsVisual -= HandleResolutionVisual;
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

                    currentView = Instantiate(prefab, targetPosition, targetRotation, spawnPoint);
                    currentView.transform.localScale = targetScale;

                    currentView.SetCard(card);
                    currentView.AllowStickers();

                    CombatEventManager.PlayerEvaluationReady(currentView.evaluationView);
                }
            );
        }
    }
}