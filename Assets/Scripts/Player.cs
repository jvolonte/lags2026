using CardZones;
using UnityEngine;

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
        while (Hand.CanDraw)
        {
            var drawnCard = Deck.Draw();
            Debug.Log($"Drawing {drawnCard} from deck");
            Hand.Add(drawnCard);
            Debug.Log($"Adding {drawnCard} to hand");
        } 
    }

    public void Play(Card card)
    {
        Hand.Play(card);
        //TODO: add it to the field

        Debug.Log($"Cards left in hand: {Hand.Count}");
    }
}