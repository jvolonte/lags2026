using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Factories
{
    public class CardFactory
    {
        readonly StickerFactory stickerFactory;

        public CardFactory(StickerFactory stickerFactory)
        {
            this.stickerFactory = stickerFactory;
        }

        public Card CreateRandom(int stickerCount = 0)
        {
            var value = Random.Range(1, 13);
            var suit = (Suit)Random.Range(0, 4);
            return Create(value, suit, stickerCount);
        }

        public Card CreateRandom(int stickerCount, int maxValue)
        {
            var value = Random.Range(1, maxValue + 1);
            var suit = (Suit)Random.Range(0, 4);
            return Create(value, suit, stickerCount);
        }

        public Card Create(int value, Suit suit, int stickerCount = 0)
        {
            var stickers = CreateStickers(stickerCount, value);
            AssignPositions(stickers);
            return new Card(value, suit, stickers);
        }

        void AssignPositions(List<StickerPlacement> stickers)
        {
            foreach (var sticker in stickers)
                sticker.LocalPosition = GetRandomPosition();
        }

        Vector2 GetRandomPosition()
        {
            const float padding = 0.15f;
            var x = Random.Range(-0.5f + padding, 0.5f - padding);
            var y = Random.Range(-0.5f + padding, 0.5f - padding);

            return new Vector2(x, y);
        }

        List<StickerPlacement> CreateStickers(int count, int cardValue)
        {
            var list = new List<StickerPlacement>(count);
            var context = new StickerContext(count, cardValue);

            for (var i = 0; i < count; i++)
            {
                var sticker = stickerFactory.CreateWithContext(context);
                context.Register(sticker);
                list.Add(sticker);
            }

            return list;
        }
    }
}