using UnityEngine;

namespace Presenters
{
    public class EnemyPresenter : MonoBehaviour
    {
        [SerializeField] Transform spawnPoint;
        [SerializeField] EnemyManager enemyManager;

        GameObject current;

        void Awake()
        {
            enemyManager.OnEnemySpawned += Spawn;
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