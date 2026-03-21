using System.Collections.Generic;
using System.Linq;

namespace CardZones
{
    public class Deck : CardZone
    {
        public void Shuffle() => Cards = Cards.Shuffle().ToList();

        public Deck(List<Card> cards) => Cards = cards;
    }
}