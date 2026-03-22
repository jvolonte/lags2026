using System;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    private int maxHealth;
    public int MaxHealth { 
        get => maxHealth;
        private set
        {
            maxHealth = value;
            OnMaxHealthChanged?.Invoke(MaxHealth);        
        } 
    }

    private int health;
    public int Health { 
        get => health; 
        private set
        {
            health = value;
            OnHealthChanged?.Invoke(Health);
        } 
    }

    public event Action<int> OnMaxHealthChanged;
    public event Action<int> OnHealthChanged;

    public void SetMaxHealth(int value)
    {
        MaxHealth = value;
        Health = MaxHealth;
    }

    public void TakeDamage(int value) => Health = Mathf.Max(0, Health - value);

    public void Heal(int value) => Health = Mathf.Min(MaxHealth, Health + value);

}