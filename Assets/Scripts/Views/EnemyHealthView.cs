using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Views
{
    public class EnemyHealthView : MonoBehaviour
    {
        [SerializeField] Transform container;
        [SerializeField] HeartView heartPrefab;

        [SerializeField] float spacing = 0.3f;
        int lastHealth = -1;

        readonly List<HeartView> hearts = new();

        void Awake()
        {
            CombatEventManager.OnEnemySet += Build;
            CombatEventManager.OnEnemyHealthChanged += UpdateHearts;
        }

        void OnDestroy()
        {
            CombatEventManager.OnEnemyHealthChanged -= UpdateHearts;
            CombatEventManager.OnEnemySet -= Build;
        }

        void Build(Enemy enemy)
        {
            lastHealth = -1;
            var maxHealth = enemy.Health;

            foreach (var h in hearts)
                Destroy(h.gameObject);

            hearts.Clear();

            var centerOffset = (maxHealth - 1) * 0.5f;

            for (var i = 0; i < maxHealth; i++)
            {
                var heart = Instantiate(heartPrefab, container);

                var x = (i - centerOffset) * spacing;
                var y = Mathf.Abs(i - centerOffset) * 0.05f;
                heart.transform.localPosition = new Vector3(x, -y, 0);
                
                var scale = heart.transform.localScale;
                heart.transform.localScale = Vector3.zero;
                heart.transform.DOScale(scale, 0.25f)
                     .SetEase(Ease.OutBack)
                     .SetDelay(i * 0.1f);

                hearts.Add(heart);
            }
            
            lastHealth = maxHealth;
        }

        void UpdateHearts(int current, int max)
        {
            Debug.Log($"HEALTH: {current}/{max}");
            
            if (lastHealth == -1)
            {
                for (var i = 0; i < hearts.Count; i++)
                    hearts[i].SetFilled(i < current);
            }
            else
            {
                if (current < lastHealth)
                {
                    var toRemove = lastHealth - current;

                    for (var i = 0; i < toRemove; i++)
                    {
                        hearts[i].SetFilled(false);
                    }
                }
                else if (current > lastHealth)
                {
                    for (var i = lastHealth; i < current; i++)
                        hearts[i].SetFilled(true);
                }
            }

            lastHealth = current;
        }
    }
}