using System;
using System.Collections.Generic;

namespace CardZones
{
    public class CardZone
    {
        public List<Card> Cards = new();
        public int Count => Cards.Count;
        public event Action<Card> OnCardAdded;
        public event Action<Card> OnCardRemoved;
        public event Action OnEmptied;

        public virtual Card Peek() => Cards[0];

        public virtual void Add(Card card)
        {
            Cards.Add(card);
            OnCardAdded?.Invoke(card);
        }

        public void TriggerCardRemoved(Card card) => OnCardRemoved?.Invoke(card);
        
        public virtual Card Draw()
        {
            if (Cards.Count == 0) return default;

            var card = Cards[0];
            Cards.RemoveAt(0);
            OnCardRemoved?.Invoke(card);
            return card;
        }

        public void Clear()
        {
            Cards.Clear();
            OnEmptied?.Invoke();
        }
    }
}