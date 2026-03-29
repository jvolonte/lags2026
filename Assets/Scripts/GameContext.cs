using System.Collections.Generic;
using System.Linq;
using Data.Stickers;
using Presenters;
using Stickers;

public class GameContext
{
    public Player Player;
    public Enemy Enemy;

    public Card EnemyCurrentCard;
    public Card PlayerCurrentCard;

    public List<StickerInstance> AvailableStickers = new();
    public WinRuleSet RuleSet;

    public int Round;

    public bool IsTutorialRound => Round == 1 && IsFirstOpponent;
    public bool IsFirstOpponent => Enemy.Data.id == EnemyId.Alfonso;
    public int WorstCardInPlayerHand => Player.Hand.Cards.Min(c => c.Value);
}

public class StickerInstance
{
    public ISticker Logic;
    public StickerData Data;
}