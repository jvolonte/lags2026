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
            var value = UnityEngine.Random.Range(1, 13);
            var suit = (Suit)UnityEngine.Random.Range(0, 4);
            var stickers = CreateStickers(stickerCount);

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

        List<StickerPlacement> CreateStickers(int count)
        {
            var list = new List<StickerPlacement>(count);

            for (var i = 0; i < count; i++)
                list.Add(stickerFactory.CreateRandomPlacement());

            return list;
        }
    }
}