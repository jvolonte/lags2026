using System.Collections;
using CardZones;
using DG.Tweening;
using Factories;
using Stickers;
using UnityEngine;
using Views;

public class GameStateManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }

    public readonly GameContext Context = new();

    readonly CardFactory cardFactory = new();
    readonly StickerFactory stickerFactory = new();
    DeckFactory deckFactory;

    EvaluationView playerEvaluationView;
    EvaluationView enemyEvaluationView;

    [Header("Views")] [SerializeField] HandView handView;
    [SerializeField] DeckView deckView;
    [SerializeField] DiscardPileView discardPileView;

    void Awake()
    {
        deckFactory = new DeckFactory(cardFactory);
        CombatEventManager.OnPlayCard += HandlePlayerSelectedCard;
        CombatEventManager.OnAddSticker += HandlePlayerSelectedSticker;

        CombatEventManager.OnEnemyEvaluationReady += v => enemyEvaluationView = v;
        CombatEventManager.OnPlayerEvaluationReady += v => playerEvaluationView = v;
        CombatEventManager.OnPlayerPlaysCard += HandlePlayerPlayCard;

        StartGame();
    }

    void HandlePlayerPlayCard(Card card)
    {
        if (CurrentState == GameState.PlayerPlaysCard && Context.PlayerCurrentCard == null)
            Context.Player.Play(card);
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
            case GameState.ConflictResolution: StartCoroutine(EnterConflictResolution()); break;
            case GameState.Draw: EnterDraw(); break;
            case GameState.GameOver: EnterGameOver(); break;
        }
    }

    void EnterSetup()
    {
        var discardPile = new DiscardPile();
        var deck = deckFactory.CreateRandom(discardPile);
        var hand = new Hand();
        var player = new Player(deck, hand, discardPile);

        deckView.Bind(deck);
        discardPileView.Bind(discardPile);
        handView.Bind(hand);

        Context.Player = player;
        Context.Enemy = new Enemy(1);
        Context.RuleSet = new WinRuleSet { HigherValueWins = true };

        Context.Player.Deck.Shuffle();

        player.Draw();

        TransitionTo(GameState.EnemyPlaysCard);
    }

    void EnterEnemyPlaysCard()
    {
        var card = cardFactory.CreateRandom();

        Context.EnemyCurrentCard = card;
        CombatEventManager.EnemyPlayCard(card);
        Debug.Log($"Enemy plays: {card}");

        TransitionTo(GameState.PlayerPlaysCard);
    }

    void EnterPlayerPlaysCard()
    {
        Debug.Log("Waiting for player input...");
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

    IEnumerator EnterConflictResolution()
    {
        var resolver = new ConflictResolver();
        var result = resolver.Resolve(Context);

        yield return PlayEvaluationPhase(
            Context.PlayerCurrentCard.Calculate(Context.EnemyCurrentCard),
            Context.EnemyCurrentCard.Calculate(Context.PlayerCurrentCard)
        );
        yield return new WaitForSeconds(2);

        resolver.ApplyOutcome(result, Context);

        TransitionTo(GameState.Draw);
    }

    IEnumerator PlayEvaluationPhase(EvaluationContext playerEval, EvaluationContext enemyEval)
    {
        yield return DOTween.Sequence()
                            .Join(playerEvaluationView.Play(playerEval))
                            .Join(enemyEvaluationView.Play(enemyEval))
                            .WaitForCompletion();
    }

    void EnterDraw()
    {
        if (OutOfCards())
        {
            TransitionTo(GameState.GameOver);
            return;
        }

        Context.Player.Draw();
        TransitionTo(GameState.EnemyPlaysCard);
    }

    bool OutOfCards() =>
        Context.Player.Hand.Count <= 0 &&
        Context.Player.Deck.Count <= 0 &&
        Context.Player.Discard.Count <= 0;

    void EnterGameOver()
    {
        Debug.Log("GAME OVER!!!");
    }

    void OnDestroy()
    {
        CombatEventManager.OnPlayCard -= HandlePlayerSelectedCard;
        CombatEventManager.OnAddSticker -= HandlePlayerSelectedSticker;
        CombatEventManager.OnPlayerPlaysCard -= HandlePlayerPlayCard;
    }
}