using UnityEngine;
using Views;

namespace Presenters
{
    public class PlayerCardPresenter : MonoBehaviour
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] CardView prefab;

        CardView currentView;

        void Awake() => 
            CombatEventManager.OnPlayCard += HandlePlayerCardPlayed;

        void OnDestroy() =>
            CombatEventManager.OnPlayCard -= HandlePlayerCardPlayed;

        void HandlePlayerCardPlayed(Card card)
        {
            if (currentView != null)
                Destroy(currentView.gameObject);

            currentView = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            currentView.SetCard(card);
        }
    }
}