using System.Collections.Generic;
using System.Linq;
using Stickers;

public class Card
{
    public int Value;
    public Suit Suit;
    public List<ISticker> Stickers;
    public int Evaluation { get; set; }

    public Card(int value, Suit suit, List<ISticker> stickers = null)
    {
        Value = value;
        Suit = suit;
        Stickers = stickers ?? new List<ISticker>();
        Evaluation = Value;
    }

    public float Calculate(Card other)
    {
        Evaluation = Value;
        
        foreach (var sticker in Stickers.OrderBy(s => s.Priority)) 
            sticker.Resolve(this, other);

        return Evaluation;
    }

    public void ApplyStickerRules(WinRuleSet ruleSet)
    {
        foreach (ISticker sticker in Stickers.OrderBy(s => s.Priority))
            sticker.ApplyRule(ruleSet);
    }

    public override string ToString() => $"{Value} of {Suit}";
}

public enum Suit
 {
     Golds,
     Cups,
     Clubs,
     Swords
 }