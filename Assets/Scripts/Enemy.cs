public class Enemy
{
    public int Health;

    public Enemy(int health)
    {
        Health = health;
    }

    public void Damage(int damage = 1) => Health -= damage;
}