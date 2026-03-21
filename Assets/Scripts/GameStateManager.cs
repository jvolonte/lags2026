using System;
using System.Collections.Generic;
using CardZones;
using Stickers;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }

    public readonly GameContext Context = new GameContext();

    void Awake()
    {
        CombatEventManager.OnPlayCard += HandlePlayerSelectedCard;
        CombatEventManager.OnAddSticker += HandlePlayerSelectedSticker;

        StartGame();
    }

    public void StartGame()
    {
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

        Context.Player = player;
        Context.Enemy = new Enemy(InitializeDeck());

        Context.Player.Deck.Shuffle();
        Context.Enemy.Deck.Shuffle();

        player.Draw();

        TransitionTo(GameState.EnemyPlaysCard);
    }

    void EnterEnemyPlaysCard()
    {
        Debug.Log("---ENEMY PLAYS CARD---");
        var enemy = Context.Enemy;
        var card = enemy.Deck.Draw();

        Context.EnemyCurrentCard = card;
        Debug.Log($"Enemy plays: {card}");

        TransitionTo(GameState.PlayerPlaysCard);
    }

    void EnterPlayerPlaysCard()
    {
        Debug.Log("---PLAYER PLAYS CARD---");
        Debug.Log("Waiting for player input.");
    }

    public void HandlePlayerSelectedCard(Card card)
    {
        if (CurrentState != GameState.PlayerPlaysCard)
            return;

        Debug.Log($"Player plays {card}");
        Context.PlayerCurrentCard = card;
        TransitionTo(GameState.RevealStickers);
    }

    void EnterRevealStickers()
    {
        Debug.Log("---REVEAL STICKERS---");

        Context.AvailableStickers.Clear();

        for (var i = 0; i < 3; i++)
            Context.AvailableStickers.Add(GetRandomSticker());

        Debug.Log($"Available stickers: {string.Join(",", Context.AvailableStickers)}");
        TransitionTo(GameState.PlayerPlaceSticker);
    }

    void EnterPlayerPlaceSticker()
    {
        Debug.Log("---PLAYER PLACES STICKER---");
        Debug.Log("Waiting for player input.");
    }

    public void HandlePlayerSelectedSticker(ISticker sticker, Card card)
    {
        if (CurrentState != GameState.PlayerPlaceSticker)
            return;

        Context.AvailableStickers.Remove(sticker);
        card.Stickers.Add(sticker);
        Debug.Log($"Adding {sticker} to {card}");

        TransitionTo(GameState.EnemyPlaceSticker);
    }

    void EnterEnemyPlaceSticker()
    {
        Debug.Log("---ENEMY PLACES STICKER---");

        //TODO: pick between available stickers!
        var random = Context.AvailableStickers.PickOne();
        Context.EnemyCurrentCard.Stickers.Add(random);
        Debug.Log($"Adding {random} to {Context.EnemyCurrentCard}");

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