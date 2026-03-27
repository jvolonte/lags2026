using System.Collections;
using Services;
using UnityEngine;
using Utils;
using Views;

namespace Presenters
{
    public class PlayerCardPresenter : MonoBehaviour
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] CardView prefab;
        [SerializeField] ViewTransitionService transitionService;
        [SerializeField] Transform previewAnchor;

        [Header("Views")] [SerializeField] DiscardPileView discardPileView;
        [SerializeField] CardCombatView combatView;

        [Header("Configuration")]
        [SerializeField]
        float combatDelay = 0.5f;

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
                currentView.SetCard(card);

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
                    currentView.gameObject.ReplaceLayerRecursively(LayerService.CARD_LAYER, LayerService.DEFAULT_LAYER);

                    currentView.SetCard(card);
                    currentView.AllowStickers();

                    StartCoroutine(InitializePreview(card));
                }
            );
        }

        IEnumerator InitializePreview(Card card)
        {
            yield return new WaitForSeconds(combatDelay);

            yield return combatView.Show(card, false);

            CombatEventManager.PlayerEvaluationReady(combatView.EvaluationView, combatView);

            CombatEventManager.PlayerCardReachedPosition();
        }
    }
}