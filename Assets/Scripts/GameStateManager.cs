using System.Collections.Generic;
using CardZones;
using Factories;
using Stickers;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }

    public readonly GameContext Context = new();

    readonly CardFactory cardFactory = new();
    readonly StickerFactory stickerFactory = new();
    DeckFactory deckFactory;

    void Awake()
    {
        deckFactory = new DeckFactory(cardFactory);
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
        Debug.Log($"STATE: {CurrentState} --> {newState}");
        CurrentState = newState;
        EnterState(newState);
    }

    void EnterState(GameState newState)
    {
        switch (newState)
        {
            case GameState.Setup: EnterSetup(); break;
            case GameState.EnemyPlaysCard: EnterEnemyPlaysCard(); break;
            case GameState.PlayerPlaysCard: EnterPlayerPlaysCard(); break;
            case GameState.RevealStickers: EnterRevealStickers(); break;
            case GameState.PlayerPlaceSticker: EnterPlayerPlaceSticker(); break;
            case GameState.EnemyPlaceSticker: EnterEnemyPlaceSticker(); break;
            case GameState.ConflictResolution: EnterConflictResolution(); break;
            case GameState.Draw: EnterDraw(); break;
        }
    }

    void EnterSetup()
    {
        var deck = deckFactory.CreateRandom();
        var hand = new Hand();
        var discardPile = new DiscardPile();
        var player = new Player(deck, hand, discardPile);

        Context.Player = player;
        Context.Enemy = new Enemy(1, InitializeDeck());

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
        Debug.Log("Waiting for player input.");
    }

    void HandlePlayerSelectedCard(Card card)
    {
        if (CurrentState != GameState.PlayerPlaysCard)
            return;

        Debug.Log($"Player plays {card}");
        Context.PlayerCurrentCard = card;
        TransitionTo(GameState.RevealStickers);
    }

    void EnterRevealStickers()
    {
        Context.AvailableStickers.Clear();

        for (var i = 0; i < 3; i++)
            Context.AvailableStickers.Add(stickerFactory.GetRandom());

        Debug.Log($"Available stickers: {string.Join(",", Context.AvailableStickers)}");

        //TODO: this might need an event from UI to transition once all stickers are shown.
        TransitionTo(GameState.PlayerPlaceSticker);
    }

    void EnterPlayerPlaceSticker()
    {
        Debug.Log("Waiting for player input.");
    }

    void HandlePlayerSelectedSticker(ISticker sticker, Card card)
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
        //TODO: pick between available stickers!
        var random = Context.AvailableStickers.PickOne();
        Context.EnemyCurrentCard.Stickers.Add(random);
        Debug.Log($"Adding {random} to {Context.EnemyCurrentCard}");

        Context.AvailableStickers.Clear();
        TransitionTo(GameState.ConflictResolution);
    }

    void EnterConflictResolution()
    {
        //TODO: handle context and cards depending on result
    }

    void EnterDraw()
    {
        Context.Player.Draw();
        TransitionTo(GameState.EnemyPlaysCard);
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

    void OnDestroy()
    {
        CombatEventManager.OnPlayCard -= HandlePlayerSelectedCard;
        CombatEventManager.OnAddSticker -= HandlePlayerSelectedSticker;
    }
}