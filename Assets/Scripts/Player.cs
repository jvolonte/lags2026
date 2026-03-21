public class Player
{
    public Deck Deck { get; private set; } = new();
    public Hand Hand { get; private set; } = new();
    public CardZone Discard { get; private set; } = new();
}