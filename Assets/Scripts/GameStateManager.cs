using System.Collections;
using System.Linq;
using CardZones;
using Data;
using Data.Stickers;
using Factories;
using Presenters;
using Services;
using UnityEngine;
using Views;

public class GameStateManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }

    public readonly GameContext Context = new();

    CardFactory cardFactory;
    StickerFactory stickerFactory;
    DeckFactory deckFactory;

    [SerializeField] EnemyManager enemyManager;

    [Header("Views")] [SerializeField] HandView handView;
    [SerializeField] DeckView deckView;
    [SerializeField] DiscardPileView discardPileView;
    [SerializeField] TooltipView tooltipView;
    [SerializeField] GameResultView gameResultView;

    [Header("Presenters")]
    [SerializeField]
    EnemyCardPresenter enemyCardPresenter;

    [SerializeField] StickerPresenter stickerPresenter;

    [Header("Stickers")] [SerializeField] StickerData[] stickers;

    bool stickerTutorial;
    bool winnerTutorial;

    EnemyTurnService enemyTurnService;
    StickerDraftService stickerDraftService;
    CombatResolutionService combatResolutionService;

    void Awake()
    {
        stickerFactory = new StickerFactory(stickers.ToList());
        cardFactory = new CardFactory(stickerFactory);
        deckFactory = new DeckFactory(cardFactory);

        enemyTurnService = new EnemyTurnService(enemyCardPresenter);
        stickerDraftService = new StickerDraftService(stickerFactory);
        combatResolutionService = new CombatResolutionService();

        CombatEventManager.OnPlayCard += HandlePlayerSelectedCard;
        CombatEventManager.OnAddSticker += HandlePlayerSelectedSticker;
        CombatEventManager.OnPlayerPlaysCard += HandlePlayerPlayCard;
        CombatEventManager.OnStickerHoverEnter += ShowTooltip;
        CombatEventManager.OnStickerHoverExit += HideTooltip;
        CombatEventManager.OnDiscard += HandleDiscard;
        CombatEventManager.OnPlayerCardReachedPosition += HandleShowStickers;
    }

    void HandleShowStickers() => TransitionTo(GameState.RevealStickers);

    void HandleDiscard(Card card) => Context.Player.Discard.Add(card);

    void ShowTooltip(StickerData data, Vector3 pos, Quaternion rot)
    {
        tooltipView.Show(data.GetDescription(), pos);
    }

    void HideTooltip() => tooltipView.Hide();

    void Start() => TransitionTo(GameState.Setup);

    void HandlePlayerPlayCard(CardView view)
    {
        if (CurrentState == GameState.PlayerPlaysCard && Context.PlayerCurrentCard == null)
            Context.Player.Play(view);
    }

    void TransitionTo(GameState newState)
    {
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
        var stickerCount = Context.Enemy.Data.stickersInCards;
        var card = cardFactory.CreateRandom(Random.Range(stickerCount.x, stickerCount.y + 1));

        Context.EnemyCurrentCard = card;
        CombatEventManager.EnemyPlayCard(card);
        TransitionTo(GameState.PlayerPlaysCard);
    }

    void EnterPlayerPlaysCard()
    {
        Debug.Log("Waiting for player to play a card...");
    }

    void HandlePlayerSelectedCard(CardView view)
    {
        if (CurrentState != GameState.PlayerPlaysCard)
            return;

        Context.PlayerCurrentCard = view.GetCard();
    }

    void EnterRevealStickers()
    {
        Context.AvailableStickers.Clear();
        Context.AvailableStickers = stickerDraftService.Draft(3);
        CombatEventManager.RevealStickers(Context.AvailableStickers);

        TransitionTo(GameState.PlayerPlaceSticker);
    }

    void EnterPlayerPlaceSticker()
    {
        if (!stickerTutorial)
        {
            DialogueService.TutorialStickerDialogue(Context.Enemy.Data);
            stickerTutorial = true;
        }

        Debug.Log("Waiting for player to place a sticker...");
    }

    void HandlePlayerSelectedSticker(StickerPlacement sticker, Card card)
    {
        if (CurrentState != GameState.PlayerPlaceSticker)
            return;

        Context.AvailableStickers = Context.AvailableStickers.Where(s => s.Logic != sticker.Logic).ToList();
        card.AddSticker(sticker);

        TransitionTo(GameState.EnemyPlaceSticker);
    }

    IEnumerator EnterEnemyPlaceSticker()
    {
        if (!winnerTutorial)
        {
            DialogueService.TutorialEncounterDialogue(Context.Enemy.Data);
            winnerTutorial = true;
        }
        else
            DialogueService.ThinkingDialogue(Context.Enemy.Data);

        yield return enemyTurnService.PlaceSticker(Context);

        yield return new WaitForSeconds(0.5f);

        Context.AvailableStickers.Clear();
        CombatEventManager.ClearStickers();
        stickerPresenter.HideSheet();

        TransitionTo(GameState.ConflictResolution);
    }

    IEnumerator EnterConflictResolution()
    {
        yield return enemyCardPresenter.MoveInPosition();
        yield return combatResolutionService.Resolve(Context);

        TransitionTo(GameState.Draw);
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
        {
            yield return new WaitForSeconds(2f);
            DialogueService.LoseGameDialogue(Context.Enemy.Data);
            yield return new WaitForSeconds(3f);
        }

        TransitionTo(Context.Enemy.IsDead ? GameState.NextEncounter : GameState.EnemyPlaysCard);
    }

    bool OutOfCards() =>
        Context.Player.Hand.Count <= 0 &&
        Context.Player.Deck.Count <= 0 &&
        Context.Player.Discard.Count <= 0;

    void EnterGameOver() => gameResultView.ShowLose();

    void EnterNextEncounter()
    {
        if (enemyManager.HasMoreEnemies)
        {
            enemyManager.StartNextEnemy();
            Context.Enemy = enemyManager.CurrentEnemy;

            TransitionTo(GameState.EnemyPlaysCard);
        }
        else
            gameResultView.ShowWin();
    }

    void OnDestroy()
    {
        CombatEventManager.OnPlayCard -= HandlePlayerSelectedCard;
        CombatEventManager.OnAddSticker -= HandlePlayerSelectedSticker;
        CombatEventManager.OnPlayerPlaysCard -= HandlePlayerPlayCard;
        CombatEventManager.OnStickerHoverEnter -= ShowTooltip;
        CombatEventManager.OnStickerHoverExit -= HideTooltip;
        CombatEventManager.OnDiscard -= HandleDiscard;
        CombatEventManager.OnPlayerCardReachedPosition -= HandleShowStickers;
    }

    public void Debug_KillEnemyAndAdvance()
    {
        Context.Enemy.Damage(Context.Enemy.Health);
        TransitionTo(GameState.NextEncounter);
    }
}