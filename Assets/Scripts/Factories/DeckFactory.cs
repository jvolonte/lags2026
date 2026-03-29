using System.Collections.Generic;
using System.Linq;
using CardZones;
using UnityEngine;

namespace Factories
{
    public class DeckFactory
    {
        readonly CardFactory cardFactory;

        public DeckFactory(CardFactory cardFactory)
        {
            this.cardFactory = cardFactory;
        }

        public Deck CreateRandom(DiscardPile discardPile, int size = 4)
        {
            var cards = new List<Card>(size);

            for (var i = 0; i < size; i++) cards.Add(cardFactory.CreateRandom());

            return new Deck(cards, discardPile);
        }

        public Deck CreatePooledDeck(DiscardPile discardPile, int size = 4)
        {
            var values = Enumerable.Range(1, 12)
                                   .OrderBy(_ => Random.value)
                                   .Take(size)
                                   .ToList();

            var suits = Enumerable.Range(0, 4)
                                  .Select(i => (Suit)i)
                                  .OrderBy(_ => Random.value)
                                  .ToList();

            var cards = new List<Card>(size);
            for (var i = 0; i < size; i++)
            {
                var value = values[i % values.Count];
                var suit = suits[i % suits.Count];
                var card = cardFactory.Create(value, suit);
                cards.Add(card);
            }

            return new Deck(cards, discardPile);
        }
    }
}