using System.Collections.Generic;
using CardZones;

namespace Factories
{
    public class DeckFactory
    {
        readonly CardFactory cardFactory;

        public DeckFactory(CardFactory cardFactory)
        {
            this.cardFactory = cardFactory;
        }

        public Deck CreateRandom(int size = 4)
        {
            var cards = new List<Card>(size);

            for (var i = 0; i < size; i++) cards.Add(cardFactory.CreateRandom());

            return new Deck(cards);
        }
    }
}