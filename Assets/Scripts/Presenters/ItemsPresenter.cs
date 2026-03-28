using UnityEngine;

namespace Presenters
{
    public class ItemsPresenter : MonoBehaviour
    {
        [SerializeField] GameObject alfonsoItems;
        [SerializeField] GameObject carmenItems;
        [SerializeField] GameObject vincentItems;
        [SerializeField] GameObject ivanItems;
        [SerializeField] GameObject baltasarItems;

        void Awake()
        {
            CombatEventManager.OnEnemyCleared += HideItems;
            CombatEventManager.OnEnemyReady += ShowItems;
        }

        void OnDestroy()
        {
            CombatEventManager.OnEnemyCleared -= HideItems;
            CombatEventManager.OnEnemyReady -= ShowItems;
        }

        void ShowItems(Enemy enemy)
        {
            var id = enemy.Data.id;
            alfonsoItems.SetActive(id == EnemyId.Alfonso);
            carmenItems.SetActive(id == EnemyId.Carmen);
            vincentItems.SetActive(id == EnemyId.Vincent);
            ivanItems.SetActive(id == EnemyId.Ivan);
            baltasarItems.SetActive(id == EnemyId.Baltasar);
        }

        void HideItems()
        {
            alfonsoItems.SetActive(false);
            carmenItems.SetActive(false);
            vincentItems.SetActive(false);
            ivanItems.SetActive(false);
            baltasarItems.SetActive(false);
        }
    }
}