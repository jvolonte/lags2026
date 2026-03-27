using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace CardZones
{
    public class Deck : CardZone
    {
        readonly DiscardPile discardPile;

        public Deck(List<Card> cards, DiscardPile discardPile)
        {
            Cards = cards;
            this.discardPile = discardPile;
        }

        public void Shuffle() => Cards = Cards.Shuffle().ToList();

        public override Card Draw()
        {
            if (Cards.Count == 0)
                RefillFromDiscard();

            if (Cards.Count == 0)
                return null;

            return base.Draw();
        }

        void RefillFromDiscard()
        {
            if (discardPile.Count == 0)
                return;

            while (discardPile.Count > 0)
            {
                var card = discardPile.Draw();
                Add(card);
            }

            Shuffle();
        }
    }
}