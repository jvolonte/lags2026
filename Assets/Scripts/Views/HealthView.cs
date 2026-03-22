using System.Collections.Generic;
using UnityEngine;

public class HealthView : MonoBehaviour
{
    [SerializeField] private HealthHandler health;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform container;

    private List<GameObject> hearts = new();

    void OnEnable() => health.OnHealthChanged += UpdateHealth;

    void OnDisable() => health.OnHealthChanged -= UpdateMaxHealth;

    void UpdateHealth(int value)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            HeartView heart = hearts[i].GetComponent<HeartView>();
            if (i < value)
                heart.Recover();
            else
                heart.Break();
        }
    }

    void UpdateMaxHealth(int value)
    {
        if (value > hearts.Count)
            for (int i = 0; i < value - hearts.Count; i++)
                hearts.Add(Instantiate(heartPrefab, container));
        else if (value < hearts.Count) 
            for (int i = 0; i < hearts.Count - value; i++)
            {                            
                int index = hearts.Count - 1;
                Destroy(hearts[index]);
                hearts.RemoveAt(index); 
            }
    }
}
