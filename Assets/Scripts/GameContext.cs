using System.Collections.Generic;
using Data;
using Data.Stickers;
using Stickers;

public class GameContext
{
    public Player Player;
    public Enemy Enemy;
    
    public Card EnemyCurrentCard;
    public Card PlayerCurrentCard;
    
    public List<StickerInstance> AvailableStickers = new();
    public WinRuleSet RuleSet;
}

public class StickerInstance
{
    public ISticker Logic;
    public StickerData Data;
}