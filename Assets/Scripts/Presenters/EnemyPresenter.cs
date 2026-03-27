using System;
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
            var delay = current != null ? 1.5f : 0;
            spawnPoint.DeleteChildren();

            if (current != null)
                Destroy(current);

            HandleBGM(enemy);
            yield return new WaitForSeconds(delay);
            current = Instantiate(enemy.Data.prefab, spawnPoint);
            CombatEventManager.EnemyReady(enemy);
            DialogueService.GameStart(enemy.Data);
        }

        static void HandleBGM(Enemy enemy)
        {
            if (enemy.Data.bgmClipId != BgmClipId.None)
                BgmManager.Play(enemy.Data.bgmClipId);
        }
    }
}