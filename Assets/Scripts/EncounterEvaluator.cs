using System.Collections.Generic;
using System.Linq;
using System;

public static class EncounterEvaluator
{

    // TODO: Contemplar el caso EMPATE

    public static Card GetWinner(Card card, Card otherCard)
    {
        WinRuleSet ruleSet = new(); 
        List<Card> cards = new() { card, otherCard };

        card.Calculate(otherCard);
        otherCard.Calculate(card);

        card.ApplyStickerRules(ruleSet);
        otherCard.ApplyStickerRules(ruleSet);

        Func<Card, Card, bool> func = ruleSet.GreaterValueWins ? IsStronger : IsWeaker;
        return cards.Aggregate((acc, c) => func(c, acc) ? c : acc); 
    }

    private static bool IsStronger(Card card, Card other) => card.Evaluation > other.Evaluation;

    private static bool IsWeaker(Card card, Card other) => card.Evaluation < other.Evaluation;
}

public enum ConflictOutcome
{
    PlayerWin,
    EnemyWin,
    Tie
}

public struct ConflictResult
{
    public int PlayerValue;
    public int EnemyValue;
    public ConflictOutcome Outcome;
}

public class WinRuleSet
{
    public bool GreaterValueWins {get; set;} = true;

}