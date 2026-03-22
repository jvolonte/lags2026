using UnityEngine;

public class ConflictResolver
{
    public ConflictResult Resolve(GameContext context)
    {
        var playerCard = context.PlayerCurrentCard;
        var enemyCard = context.EnemyCurrentCard;

        var playerEvaluationContext = playerCard.Calculate(enemyCard);
        var enemyEvaluationContext = enemyCard.Calculate(playerCard);

        var outcome = EncounterEvaluator.DetermineOutcome(
            playerEvaluationContext.Value,
            enemyEvaluationContext.Value,
            playerCard,
            enemyCard,
            context.RuleSet
        );

        return new ConflictResult
        {
            PlayerValue = playerEvaluationContext.Value,
            EnemyValue = enemyEvaluationContext.Value,
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
        Debug.Log("---PLAYER WON---");
        context.Enemy.Damage();

        context.Player.Discard.Add(context.PlayerCurrentCard);
        context.Player.Discard.Add(context.EnemyCurrentCard);
        Cleanup(context);
    }

    void ResolveEnemyWin(ConflictResult result, GameContext context)
    {
        Debug.Log("---ENEMY WON---");
        Cleanup(context);
    }

    void ResolveTie(ConflictResult result, GameContext context)
    {
        Debug.Log("---TIE---");
        context.Player.Discard.Add(context.PlayerCurrentCard);
        Cleanup(context);
    }

    void Cleanup(GameContext context)
    {
        context.PlayerCurrentCard = null;
        context.EnemyCurrentCard = null;
        context.AvailableStickers.Clear();
        CombatEventManager.ClearTable();
    }
}