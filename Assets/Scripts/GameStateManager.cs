using System;
using System.Collections.Generic;
using CardZones;
using Stickers;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }

    GameContext context;

    void Awake() => StartGame();

    public void StartGame()
    {
        context = new GameContext();
        TransitionTo(GameState.Setup);
    }

    void TransitionTo(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Setup:
                EnterSetup();
                break;
            case GameState.EnemyPlaysCard:
                EnterEnemyPlaysCard();
                break;
            case GameState.PlayerPlaysCard:
                EnterPlayerPlaysCard();
                break;
            case GameState.RevealStickers:
                EnterRevealStickers();
                break;
            case GameState.PlayerPlaceSticker:
                EnterPlayerPlaceSticker();
                break;
            case GameState.EnemyPlaceSticker:
                EnterEnemyPlaceSticker();
                break;
            case GameState.ConflictResolution:
                EnterConflictResolution();
                break;
            case GameState.Draw:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    //TODO: this random is being run in the same frame so its all the same.
    //Need to find a better way to get these
    ISticker GetRandomSticker()
    {
        List<ISticker> stickers =
            new()
            {
                new AdditiveSticker(UnityEngine.Random.Range(1, 10)),
                new AdditiveSticker(UnityEngine.Random.Range(1, 10)),
                new MultiplierSticker(1.5f),
                new MultiplierSticker(2f),
            };

        return stickers.PickOne();
    }

    void EnterSetup()
    {
        Debug.Log("---SETUP---");
        var deck = InitializeDeck();
        var hand = new Hand();
        var discardPile = new DiscardPile();
        var player = new Player(deck, hand, discardPile);

        context.Player = player;
        context.Enemy = new Enemy(InitializeDeck());

        context.Player.Deck.Shuffle();
        context.Enemy.Deck.Shuffle();

        player.Draw();

        TransitionTo(GameState.EnemyPlaysCard);
    }

    void EnterEnemyPlaysCard()
    {
        Debug.Log("---ENEMY PLAYS CARD---");
        var enemy = context.Enemy;
        var card = enemy.Deck.Draw();

        context.EnemyCurrentCard = card;
        Debug.Log($"Enemy plays: {card}");

        TransitionTo(GameState.PlayerPlaysCard);
    }

    void EnterPlayerPlaysCard()
    {
        Debug.Log("---PLAYER PLAYS CARD---");
        
        //TODO: Wait for player input
        
        Debug.Log(" >>> Faking player input");
        var player = context.Player;
        var card = player.Hand.Cards.PickOne();
        
        HandlePlayerSelectedCard(card);
    }

    public void HandlePlayerSelectedCard(Card card)
    {
        if (CurrentState != GameState.PlayerPlaysCard)
            return;

        context.PlayerCurrentCard = card;
        TransitionTo(GameState.RevealStickers);
    }

    void EnterRevealStickers()
    {
        Debug.Log("---REVEAL STICKERS---");

        context.AvailableStickers.Clear();

        for (var i = 0; i < 3; i++)
            context.AvailableStickers.Add(GetRandomSticker());

        Debug.Log($"Available stickers: {string.Join(",", context.AvailableStickers)}");
        TransitionTo(GameState.PlayerPlaceSticker);
    }

    void EnterPlayerPlaceSticker()
    {
        Debug.Log("---PLAYER PLACES STICKER---");
        //TODO: Wait for player input

        Debug.Log(" >>> Faking player input");
        var sticker = context.AvailableStickers.PickOne();
        HandlePlayerSelectedSticker(sticker);
    }

    public void HandlePlayerSelectedSticker(ISticker sticker)
    {
        if (CurrentState != GameState.PlayerPlaceSticker)
            return;

        context.AvailableStickers.Remove(sticker);
        context.PlayerCurrentCard.Stickers.Add(sticker);
        Debug.Log($"Adding {sticker} to {context.PlayerCurrentCard}");

        TransitionTo(GameState.EnemyPlaceSticker);
    }

    void EnterEnemyPlaceSticker()
    {
        Debug.Log("---ENEMY PLACES STICKER---");
        
        //TODO: pick between available stickers!
        var random = context.AvailableStickers.PickOne();
        context.EnemyCurrentCard.Stickers.Add(random);
        Debug.Log($"Adding {random} to {context.EnemyCurrentCard}");

        TransitionTo(GameState.ConflictResolution);
    }

    void EnterConflictResolution()
    {
        Debug.Log("---CONFLICT RESOLUTION---");

        //TODO: handle context and cards depending on result
    }

    static Deck InitializeDeck() =>
        new(
            new List<Card>
            {
                new(UnityEngine.Random.Range(1, 13), Suit.Cups),
                new(UnityEngine.Random.Range(1, 13), Suit.Swords),
                new(UnityEngine.Random.Range(1, 13), Suit.Clubs),
                new(UnityEngine.Random.Range(1, 13), Suit.Golds),
            }
        );
}