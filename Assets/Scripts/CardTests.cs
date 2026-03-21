using System.Collections.Generic;
using System.Linq;
using CardZones;
using Stickers;
using UnityEngine;

public class CardTests : MonoBehaviour
{
    void Awake()
    {
        EvaluationTest();
        
        PlayCardFromHandTest();
    }

    static void PlayCardFromHandTest()
    {
        var deck = new Deck(
            new List<Card>
            {
                new(10, Suit.Cups),
                new(3, Suit.Clubs),
                new(1, Suit.Swords),
                new(8, Suit.Swords),
            }
        );
        deck.Shuffle();

        var hand = new Hand();
        var discardPile = new DiscardPile();
        var player = new Player(deck, hand, discardPile);
        player.Draw();

        player.Play(hand.Cards.First());
    }

    static void EvaluationTest()
    {
        var stickers = new List<ISticker> { new AdditiveSticker(3), new MultiplierSticker(1.5f) };
        var card = new Card(10, Suit.Cups, stickers);
        var otherCard = new Card(3, Suit.Golds);

        var cardEvaluation = card.Calculate(otherCard);
        var otherCardEvaluation = otherCard.Calculate(card);

        Debug.Log($"Playing {card} against {otherCard}. Evaluation: {cardEvaluation}. Other: {otherCardEvaluation}");
    }
}