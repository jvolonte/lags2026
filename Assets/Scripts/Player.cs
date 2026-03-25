using CardZones;
using Views;

public class Player
{
    public Deck Deck { get; private set; } 
    public Hand Hand { get; private set; }
    public DiscardPile Discard { get; private set; }

    public Player(Deck deck, Hand hand, DiscardPile discard)
    {
        Deck = deck;
        Hand = hand;
        Discard = discard;
    }

    public void Draw()
    {
        while (Hand.CanDraw && (Deck.Count > 0 || Discard.Count > 0))
        {
            var drawnCard = Deck.Draw();
            Hand.Add(drawnCard);
        } 
    }

    public void Play(CardView view) => Hand.Play(view);
}