using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using DG.Tweening;
using Services;
using UnityEngine;
using Views;

namespace Presenters
{
    public class EnemyCardPresenter : MonoBehaviour
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] CardView prefab;
        [SerializeField] ViewTransitionService transitionService;
        [SerializeField] DiscardPileView discardPileView;

        [SerializeField] Transform previewAnchor;
        [SerializeField] Transform combatAnchor;
        [SerializeField] CardCombatView combatView;

        CardView currentView;

        void Awake()
        {
            CombatEventManager.OnEnemyPlayCard += HandleEnemyCardPlayed;
            CombatEventManager.OnEnemyPlaceStickerPreview += HandleEnemyPlaceStickerPreview;
            CombatEventManager.OnResolveCardsVisual += HandleResolutionVisual;

            //TODO: clean up. This is to avoid the navigation from behind and into "combat"
            previewAnchor = combatAnchor;
        }

        void HandleResolutionVisual(GameContext game, ResolutionContext resolution, ConflictOutcome conflictOutcome)
        {
            //if (previewView == null)
            //    return;

            var card = combatView.GetCard();

            if (card == null) return;

            var fate = GetFinalFate(conflictOutcome, resolution.Get(card));
            StartCoroutine(AnimateResolution(fate));
        }

        CardFate GetFinalFate(ConflictOutcome round, CardOutcome effect) =>
            effect.Destroy || effect.WillBeLost
                ? CardFate.Destroy
                : round switch
                {
                    ConflictOutcome.PlayerWin => CardFate.Discard,
                    ConflictOutcome.Tie => CardFate.Destroy,
                    ConflictOutcome.EnemyWin => CardFate.Destroy,
                    _ => CardFate.Destroy
                };

        IEnumerator AnimateResolution(CardFate fate)
        {
            combatView.Hide(true);

            if (fate is CardFate.Discard)
            {
                var target = discardPileView.GetAnchor();

                yield return transitionService.MoveAndSwap(
                    source: currentView.transform,
                    target: target,
                    proxyPrefab: currentView.gameObject,
                    onArrive: () => CombatEventManager.Discard(currentView.GetCard())
                );
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

        void OnDestroy()
        {
            CombatEventManager.OnEnemyPlayCard -= HandleEnemyCardPlayed;
            CombatEventManager.OnEnemyPlaceStickerPreview -= HandleEnemyPlaceStickerPreview;
            CombatEventManager.OnResolveCardsVisual -= HandleResolutionVisual;
        }

        void HandleEnemyCardPlayed(Card card)
        {
            if (currentView != null)
                Destroy(currentView.gameObject);

            currentView = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            currentView.SetCard(card);

            StartCoroutine(InitializePreview(card));
        }

        IEnumerator InitializePreview(Card card)
        {
            yield return new WaitForSeconds(.5f);

            //previewView = Instantiate(prefab, currentView.transform.position, currentView.transform.rotation);
            //previewView.SetCard(card);

            //previewView.transform.localScale = Vector3.zero;
            //previewView.transform
            //           .DOScale(2f, 0.25f)
            //           .SetEase(Ease.OutBack);

            //yield return transitionService.Move(
            //    previewView.transform,
            //    previewAnchor.position,
            //    previewAnchor.rotation,
            //    1f
            //);

            //previewView.transform.SetParent(previewAnchor);

            yield return combatView.Show(card, true);

            CombatEventManager.EnemyEvaluationReady(combatView.EvaluationView, combatView);
        }

        void HandleEnemyPlaceStickerPreview(Card card, StickerPlacement placement)
        {
            if (currentView == null)
                return;

            currentView.SetCard(card);
            combatView.SetCard(card, false);
        }

        public Vector2 GetRandomStickerPosition() =>
            GetEnemyStickerPosition(currentView.transform, currentView.GetCard().Stickers);

        static Vector2 GetEnemyStickerPosition(Transform container, List<StickerPlacement> existing)
        {
            var renderer = container.GetComponentInChildren<Renderer>();
            var bounds = renderer.bounds;

            var minLocal = container.InverseTransformPoint(bounds.min);
            var maxLocal = container.InverseTransformPoint(bounds.max);

            var padding = 0.1f;
            var minDistance = 0.3f;

            float RandomCentered() => (Random.value + Random.value) * 0.5f;

            for (var i = 0; i < 10; i++)
            {
                var tx = RandomCentered();
                var ty = RandomCentered();

                var x = Mathf.Lerp(minLocal.x + padding, maxLocal.x - padding, tx);
                var y = Mathf.Lerp(minLocal.y + padding, maxLocal.y - padding, ty);

                var candidate = new Vector2(x, y);

                var tooClose = existing.Any(s =>
                    Vector2.Distance(candidate, s.LocalPosition) < minDistance
                );

                if (!tooClose)
                    return candidate;
            }

            return (minLocal + maxLocal) * 0.5f;
        }

        public IEnumerator MoveInPosition()
        {
            //if (previewView == null)
            //    yield break;

            //yield return transitionService.Move(
            //    previewView.transform,
            //    combatAnchor.position,
            //    combatAnchor.rotation,
            //    1f
            //);

            //previewView.ShowEvaluation();
            yield break;
        }
    }

    public enum CardFate
    {
        Discard,
        Destroy,
    }
}