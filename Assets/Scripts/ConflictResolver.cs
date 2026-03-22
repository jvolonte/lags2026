using UnityEngine;

public class ConflictResolver
{
    public ConflictResult Resolve(Card playerCard, Card enemyCard)
    {
        var playerValue = Mathf.FloorToInt(playerCard.Calculate(enemyCard));
        var enemyValue = Mathf.FloorToInt(enemyCard.Calculate(playerCard));

        var outcome = DetermineOutcome(playerValue, enemyValue);

        return new ConflictResult
        {
            PlayerValue = playerValue,
            EnemyValue = enemyValue,
            Outcome = outcome
        };
    }

    ConflictOutcome DetermineOutcome(int playerValue, int enemyValue)
    {
        //TODO: should use encounter evaluator 
        if (playerValue > enemyValue)
            return ConflictOutcome.PlayerWin;

        if (enemyValue > playerValue)
            return ConflictOutcome.EnemyWin;

        return ConflictOutcome.Tie;
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
        context.Enemy.Damage();
        
        context.Player.Discard.Add(context.PlayerCurrentCard);
        context.Player.Discard.Add(context.EnemyCurrentCard);
    }

    void ResolveEnemyWin(ConflictResult result, GameContext context)
    {
        
    }

    void ResolveTie(ConflictResult result, GameContext context)
    {
        context.Player.Discard.Add(context.PlayerCurrentCard);
    }
}