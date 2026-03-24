using System.Collections;
using System.Linq;
using CardZones;
using Data;
using Data.Stickers;
using DG.Tweening;
using Factories;
using Presenters;
using UnityEngine;
using Views;

public class GameStateManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }

    public readonly GameContext Context = new();

    readonly CardFactory cardFactory = new();
    StickerFactory stickerFactory;
    DeckFactory deckFactory;

    EvaluationView playerEvaluationView;
    EvaluationView enemyEvaluationView;

    [SerializeField] EnemyManager enemyManager;

    [Header("Views")] [SerializeField] HandView handView;
    [SerializeField] DeckView deckView;
    [SerializeField] DiscardPileView discardPileView;

    [Header("Presenters")] 
    [SerializeField] EnemyCardPresenter enemyCardPresenter;

    [Header("Stickers")] [SerializeField] StickerData[] stickers;

    void Awake()
    {
        stickerFactory = new StickerFactory(stickers.ToList());
        deckFactory = new DeckFactory(cardFactory);
        CombatEventManager.OnPlayCard += HandlePlayerSelectedCard;
        CombatEventManager.OnAddSticker += HandlePlayerSelectedSticker;

        CombatEventManager.OnEnemyEvaluationReady += v => enemyEvaluationView = v;
        CombatEventManager.OnPlayerEvaluationReady += v => playerEvaluationView = v;
        CombatEventManager.OnPlayerPlaysCard += HandlePlayerPlayCard;
    }

    void Start() => TransitionTo(GameState.Setup);

    void HandlePlayerPlayCard(Card card)
    {
        if (CurrentState == GameState.PlayerPlaysCard && Context.PlayerCurrentCard == null)
            Context.Player.Play(card);
    }

    void TransitionTo(GameState newState)
    {
        // Debug.Log($"STATE: {CurrentState} --> {newState}");
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
            case GameState.EnemyPlaceSticker: StartCoroutine(EnterEnemyPlaceSticker()); break;
            case GameState.ConflictResolution: StartCoroutine(EnterConflictResolution()); break;
            case GameState.Draw: StartCoroutine(EnterDraw()); break;
            case GameState.GameOver: EnterGameOver(); break;
            case GameState.NextEncounter: EnterNextEncounter(); break;
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

        enemyManager.StartNextEnemy();
        Context.Enemy = enemyManager.CurrentEnemy;
        Context.RuleSet = new WinRuleSet { HigherValueWins = true };

        Context.Player.Deck.Shuffle();

        player.Draw();

        TransitionTo(GameState.EnemyPlaysCard);
    }

    void EnterEnemyPlaysCard()
    {
        //TODO: use enemies cards?
        var card = cardFactory.CreateRandom();

        Context.EnemyCurrentCard = card;
        CombatEventManager.EnemyPlayCard(card);

        TransitionTo(GameState.PlayerPlaysCard);
    }

    void EnterPlayerPlaysCard()
    {
        Debug.Log("Waiting for player to play a card...");
    }

    void HandlePlayerSelectedCard(Card card)
    {
        if (CurrentState != GameState.PlayerPlaysCard)
            return;

        Context.PlayerCurrentCard = card;
        TransitionTo(GameState.RevealStickers);
    }

    void EnterRevealStickers()
    {
        Context.AvailableStickers.Clear();

        for (var i = 0; i < 3; i++)
        {
            var (logic, data) = stickerFactory.GetRandom();

            Context.AvailableStickers.Add(new StickerInstance
            {
                Logic = logic,
                Data = data
            });
        }

        CombatEventManager.RevealStickers(Context.AvailableStickers);

        TransitionTo(GameState.PlayerPlaceSticker);
    }

    void EnterPlayerPlaceSticker()
    {
        Debug.Log("Waiting for player to place a sticker...");
    }

    void HandlePlayerSelectedSticker(StickerPlacement sticker, Card card)
    {
        if (CurrentState != GameState.PlayerPlaceSticker)
            return;

        Context.AvailableStickers = Context.AvailableStickers.Where(s => s.Logic != sticker.Logic).ToList();
        card.Stickers.Add(sticker);

        TransitionTo(GameState.EnemyPlaceSticker);
    }

    IEnumerator EnterEnemyPlaceSticker()
    {
        //TODO: show a thinking dialogue here
        yield return new WaitForSeconds(1);

        //TODO: pick between available stickers!
        var random = Context.AvailableStickers.PickOne();
        var placement = new StickerPlacement
        {
            Logic = random.Logic,
            Data = random.Data,
            LocalPosition = enemyCardPresenter.GetRandomStickerPosition()
        };

        Context.EnemyCurrentCard.Stickers.Add(placement);
        CombatEventManager.OnEnemyPlaceStickerPreview?.Invoke(Context.EnemyCurrentCard, placement);

        yield return new WaitForSeconds(0.5f);

        Context.AvailableStickers.Clear();
        CombatEventManager.ClearStickers();
        TransitionTo(GameState.ConflictResolution);
    }

    IEnumerator EnterConflictResolution()
    {
        var resolver = new ConflictResolver();
        var result = resolver.Resolve(Context);

        yield return PlayEvaluationPhase(
            Context.PlayerCurrentCard.Calculate(Context.EnemyCurrentCard, Context),
            Context.EnemyCurrentCard.Calculate(Context.PlayerCurrentCard, Context)
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

    IEnumerator EnterDraw()
    {
        if (OutOfCards())
        {
            TransitionTo(GameState.GameOver);
            yield break;
        }

        Context.Player.Draw();

        if (Context.Enemy.IsDead)
            yield return new WaitForSeconds(1);

        TransitionTo(Context.Enemy.IsDead ? GameState.NextEncounter : GameState.EnemyPlaysCard);
    }

    bool OutOfCards() =>
        Context.Player.Hand.Count <= 0 &&
        Context.Player.Deck.Count <= 0 &&
        Context.Player.Discard.Count <= 0;

    void EnterGameOver()
    {
        Debug.Log("GAME OVER!!!");
    }

    void EnterNextEncounter()
    {
        if (enemyManager.HasMoreEnemies)
        {
            enemyManager.StartNextEnemy();
            Context.Enemy = enemyManager.CurrentEnemy;

            TransitionTo(GameState.EnemyPlaysCard);
        }
        else
        {
            Debug.Log("GAME WON!");
        }
    }

    void OnDestroy()
    {
        CombatEventManager.OnPlayCard -= HandlePlayerSelectedCard;
        CombatEventManager.OnAddSticker -= HandlePlayerSelectedSticker;
        CombatEventManager.OnPlayerPlaysCard -= HandlePlayerPlayCard;
    }

    public void Debug_KillEnemyAndAdvance()
    {
        Context.Enemy.Damage(Context.Enemy.Health);
        TransitionTo(GameState.NextEncounter);
    }
}