using Data;

public class Enemy
{
    public EnemyData Data { get; }

    public int Health { get; private set; }

    public Enemy(EnemyData data)
    {
        Data = data;
        Health = data.health;
    }

    public void Damage(int amount = 1)
    {
        Health -= amount;
    }

    public bool IsDead => Health <= 0;
}