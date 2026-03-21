using UnityEngine;
using Views;

namespace Presenters
{
    public class EnemyCardPresenter : MonoBehaviour
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] CardView prefab;

        CardView currentView;

        void Awake() => 
            CombatEventManager.OnEnemyPlayCard += HandleEnemyCardPlayed;

        void OnDestroy() =>
            CombatEventManager.OnEnemyPlayCard -= HandleEnemyCardPlayed;

        void HandleEnemyCardPlayed(Card card)
        {
            if (currentView != null)
                Destroy(currentView.gameObject);

            currentView = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            currentView.SetCard(card);
        }
    }
}