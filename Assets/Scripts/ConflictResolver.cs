using UnityEngine;

public class ConflictResolver
{
    public ConflictResult Resolve(GameContext context)
    {
        var playerCard = context.PlayerCurrentCard;
        var enemyCard = context.EnemyCurrentCard;

        var playerValue = Mathf.FloorToInt(playerCard.Calculate(enemyCard));
        var enemyValue = Mathf.FloorToInt(enemyCard.Calculate(playerCard));

        var outcome = EncounterEvaluator.DetermineOutcome(
            playerValue, enemyValue, playerCard, enemyCard, context.RuleSet
        );

        return new ConflictResult
        {
            PlayerValue = playerValue,
            EnemyValue = enemyValue,
            Outcome = outcome
        };
    }

    public void ApplyOutcome(ConflictResult result, GameContext context)
    {
        switch (result.Outcome)
        {
            case ConflictOutcome.PlayerWin: ResolvePlayerWin(result, context); break;
            case ConflictOutcome.EnemyWin: ResolveEnemyWin(result, context); break;
            case ConflictOutcome.Tie: ResolveTie(result, context); break;
        }
    }

    void ResolvePlayerWin(ConflictResult result, GameContext context)
    {
        Debug.Log("PLAYER WON");
        context.Enemy.Damage();

        context.Player.Discard.Add(context.PlayerCurrentCard);
        context.Player.Discard.Add(context.EnemyCurrentCard);
    }

    void ResolveEnemyWin(ConflictResult result, GameContext context)
    {
        Debug.Log("ENEMY WON");
    }

    void ResolveTie(ConflictResult result, GameContext context)
    {
        Debug.Log("IT'S A TIE");
        context.Player.Discard.Add(context.PlayerCurrentCard);
    }
}