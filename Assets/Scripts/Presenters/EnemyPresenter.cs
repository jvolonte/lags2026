using System.Collections;
using Audio;
using Services;
using UnityEngine;
using Utils;

namespace Presenters
{
    public class EnemyPresenter : MonoBehaviour
    {
        [SerializeField] Transform spawnPoint;
        GameObject current;
        TimeOfDay currentTimeOfDay;

        void Awake()
        {
            CombatEventManager.OnEnemySet += Spawn;
        }

        void OnDestroy()
        {
            CombatEventManager.OnEnemySet -= Spawn;
        }

        void Spawn(Enemy enemy)
        {
            StartCoroutine(SpawnCoroutine(enemy));
        }

        IEnumerator SpawnCoroutine(Enemy enemy)
        {
            yield return new WaitForSeconds(1);
            var delay = current != null ? 1.5f : 0;
            spawnPoint.DeleteChildren();
            
            if (current != null)
            {
                Destroy(current);
                CombatEventManager.ClearEnemy();
            }

            HandleTimeOfDay(enemy);

            HandleBGM(enemy);
            yield return new WaitForSeconds(delay);
            current = Instantiate(enemy.Data.prefab, spawnPoint);
            CombatEventManager.EnemyReady(enemy);
            DialogueService.GameStart(enemy.Data, isTutorial: enemy.Data.id == "Alfonso");
        }

        void HandleTimeOfDay(Enemy enemy)
        {
            if (enemy.Data.TimeOfDay != currentTimeOfDay)
            {
                currentTimeOfDay = enemy.Data.TimeOfDay;
                CombatEventManager.ChangeTimeOfDay(currentTimeOfDay);
            }
        }
        

        static void HandleBGM(Enemy enemy)
        {
            if (enemy.Data.bgmClipId != BgmClipId.None)
                BgmManager.Play(enemy.Data.bgmClipId);
        }
    }
}