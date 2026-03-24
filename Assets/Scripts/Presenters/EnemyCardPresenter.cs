using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using Views;

namespace Presenters
{
    public class EnemyCardPresenter : MonoBehaviour
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] CardView prefab;

        CardView currentView;

        void Awake()
        {
            CombatEventManager.OnEnemyPlayCard += HandleEnemyCardPlayed;
            CombatEventManager.OnClearTable += HandleClearTable;
            CombatEventManager.OnEnemyPlaceStickerPreview += HandleEnemyPlaceStickerPreview;
        }

        void HandleClearTable()
        {
            if (currentView != null)
                Destroy(currentView.gameObject);
        }

        void OnDestroy()
        {
            CombatEventManager.OnEnemyPlayCard -= HandleEnemyCardPlayed;
            CombatEventManager.OnClearTable -= HandleClearTable;
            CombatEventManager.OnEnemyPlaceStickerPreview -= HandleEnemyPlaceStickerPreview;
        }

        void HandleEnemyCardPlayed(Card card)
        {
            if (currentView != null)
                Destroy(currentView.gameObject);

            currentView = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            currentView.SetCard(card);

            CombatEventManager.EnemyEvaluationReady(currentView.evaluationView);
        }

        void HandleEnemyPlaceStickerPreview(Card card, StickerPlacement placement)
        {
            if (currentView == null)
                return;

            currentView.SetCard(card);
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

            for (var i = 0; i < 10; i++)
            {
                var x = Random.Range(minLocal.x + padding, maxLocal.x - padding);
                var y = Random.Range(minLocal.y + padding, maxLocal.y - padding);

                var candidate = new Vector2(x, y);

                var tooClose = existing.Any(s =>
                    Vector2.Distance(candidate, s.LocalPosition) < minDistance
                );

                if (!tooClose)
                    return candidate;
            }

            return Vector2.zero;
        }
    }
}