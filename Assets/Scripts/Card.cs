using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class Card
{
    public int Value;
    public Suit Suit;
    public List<StickerPlacement> Stickers;
    public int Evaluation { get; set; }

    public Card(int value, Suit suit, List<StickerPlacement> stickers = null)
    {
        Value = value;
        Suit = suit;
        Stickers = stickers ?? new List<StickerPlacement>();
        Evaluation = Value;
    }

    public EvaluationContext Calculate(Card other)
    {
        Debug.Log($"Calculating card {this} with {Stickers.Count} stickers. Stickers: {string.Join(",", Stickers)}");
        
        var context = new EvaluationContext { Value = Value };
        context.AddStep(Value, "Base", StepType.Base);

        foreach (var sticker in Stickers.Select(s => s.Logic).OrderBy(s => s.Priority))
            sticker.Resolve(context, this, other);

        return context;
    }

    public void ApplyStickerRules(WinRuleSet ruleSet)
    {
        foreach (var sticker in Stickers.Select(s => s.Logic).OrderBy(s => s.Priority))
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

public struct EvaluationStep
{
    public int PreviousValue;
    public int NewValue;
    public string Description;
    public StepType Type;
}

public enum StepType
{
    Base,
    Add,
    Multiply,
    Conditional,
    Critical
}