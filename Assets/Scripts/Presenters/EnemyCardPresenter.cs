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
    }
}