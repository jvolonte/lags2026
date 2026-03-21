using System.Collections.Generic;
using Stickers;
using UnityEngine;

public class CardTests : MonoBehaviour
{
    void Awake()
    {
        var stickers = new List<ISticker> { new AdditiveSticker(3), new MultiplierSticker(1.5f) };
        var card = new Card(10, Suit.Cups, stickers);
        var otherCard = new Card(3, Suit.Golds,  new List<ISticker>());

        var cardEvaluation = card.Calculate(otherCard);
        var otherCardEvaluation = otherCard.Calculate(card);

        Debug.Log($"Playing {card} against {otherCard}. Evaluation: {cardEvaluation}. Other: {otherCardEvaluation}");
    }
}