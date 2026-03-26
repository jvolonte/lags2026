using System.Collections.Generic;
using Data;
using Factories;
using Services;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] List<EnemyData> enemies;

    int currentIndex = -1;
    public Enemy CurrentEnemy { get; private set; }

    public void StartNextEnemy()
    {
        currentIndex++;

        if (currentIndex >= enemies.Count)
        {
            Debug.Log("All enemies defeated");
            return;
        }

        var data = enemies[currentIndex];

        CurrentEnemy = EnemyFactory.Create(data);
        DialogueService.GameStart(CurrentEnemy.Data);
        CombatEventManager.SetEnemy(CurrentEnemy);
    }

    public bool HasMoreEnemies => currentIndex < enemies.Count - 1;
}