using UnityEngine;

namespace Presenters
{
    public class EnemyPresenter : MonoBehaviour
    {
        [SerializeField] Transform spawnPoint;
        GameObject current;

        void Awake()
        {
            CombatEventManager.OnEnemySet += Spawn;
        }

        void Spawn(Enemy enemy)
        {
            spawnPoint.DeleteChildren();

            if (current != null)
                Destroy(current);

            current = Instantiate(enemy.Data.prefab, spawnPoint);
        }
    }
}