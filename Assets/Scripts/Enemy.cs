public class Enemy
{    
    public int Health { get; private set; }

    public Enemy(int health)
    {
        Health = health;
    }

    public void Damage(int damage = 1) => Health -= damage;
}