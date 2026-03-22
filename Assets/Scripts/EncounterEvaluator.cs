using UnityEngine;
using System.Collections.Generic;

public static class EncounterEvaluator
{
    public static Card GetWinner(Card card, Card otherCard)
    {
        WinRuleSet ruleSet = new(); 
        List<Card> cards = new() { card, otherCard };

        card.Calculate(otherCard);
        otherCard.Calculate(card);

        card.ApplyStickerRules(ruleSet);
        otherCard.ApplyStickerRules(ruleSet);

        return ruleSet.GreaterValueWins ? cards.MaxBy(x => x.Evaluation) : cards.MinBy(x => x.Evaluation);
    }
}

public class WinRuleSet
{
    bool GreaterValueWins {get; set;} = true;

}