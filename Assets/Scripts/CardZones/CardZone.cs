using System.Collections.Generic;

namespace CardZones
{
    public class CardZone
    {
        public List<Card> Cards = new();
        public int Count => Cards.Count;
        public Card Peek() => Cards[0];
        
        public virtual void Add(Card card) => Cards.Add(card);

        public virtual Card Draw()
        {
            if (Cards.Count == 0) return default;

            var card = Cards[0];
            Cards.RemoveAt(0);
            return card;
        }
    }
}