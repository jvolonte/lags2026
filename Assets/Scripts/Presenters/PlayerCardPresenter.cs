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

        CardView currentView;

        void Awake()
        {
            CombatEventManager.OnPlayCard += HandlePlayerCardPlayed;
            CombatEventManager.OnClearTable += HandleClearTable;
        }

        void HandleClearTable()
        {
            if (currentView != null) 
                Destroy(currentView.gameObject);
        }

        void OnDestroy()
        {
            CombatEventManager.OnPlayCard -= HandlePlayerCardPlayed;
            CombatEventManager.OnClearTable -= HandleClearTable;
        }

        void HandlePlayerCardPlayed(CardView source)
        {
            StartCoroutine(PlayCardTransition(source.GetCard(), source));
        }

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