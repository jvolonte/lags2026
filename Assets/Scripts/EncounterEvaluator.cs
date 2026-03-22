public static class EncounterEvaluator
{
    public static ConflictOutcome DetermineOutcome(
        int playerValue, int enemyValue, Card playerCard, Card enemyCard, WinRuleSet ruleSet
    )
    {
        playerCard.ApplyStickerRules(ruleSet);
        enemyCard.ApplyStickerRules(ruleSet);

        if (playerValue > enemyValue)
            return ruleSet.HigherValueWins
                ? ConflictOutcome.PlayerWin
                : ConflictOutcome.EnemyWin;

        if (enemyValue > playerValue)
            return ruleSet.HigherValueWins
                ? ConflictOutcome.EnemyWin
                : ConflictOutcome.PlayerWin;

        return ConflictOutcome.Tie;
    }
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
    public bool HigherValueWins { get; set; } = true;
}