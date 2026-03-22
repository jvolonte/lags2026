using System.Collections.Generic;
using Stickers;

public class GameContext
{
    public Player Player;
    public Enemy Enemy;
    
    public Card EnemyCurrentCard;
    public Card PlayerCurrentCard;
    
    public List<ISticker> AvailableStickers = new();
    public WinRuleSet RuleSet;
}