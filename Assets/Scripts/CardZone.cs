using System.Collections.Generic;

public class CardZone
{
    protected List<Card> cards = new();
    public IEnumerable<Card> Cards => cards;

    public virtual void Add(Card card) => cards.Add(card);

    public virtual Card Draw()
    {
        if (cards.Count == 0) return default;

        var card = cards[0];
        cards.RemoveAt(0);
        return card;
    }
}