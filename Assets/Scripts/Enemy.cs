using CardZones;

public class Enemy
{
    public int Health;
    public Deck Deck;

    public Enemy(int health, Deck deck)
    {
        Health = health;
        Deck = deck;
    }

    public void Damage(int damage = 1) => Health -= damage;
}