using System.Linq;
using Utils;

public class ConflictResolver
{
    public static ConflictResult Resolve(GameContext context)
    {
        var playerCard = context.PlayerCurrentCard;
        var enemyCard = context.EnemyCurrentCard;

        var playerEvaluationContext = playerCard.Calculate(enemyCard, context);
        var enemyEvaluationContext = enemyCard.Calculate(playerCard, context);

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
            Outcome = outcome,
            PlayerEvaluation = playerEvaluationContext,
            EnemyEvaluation = enemyEvaluationContext
        };
    }

    public static void ApplyOutcome(ConflictResult result, GameContext game)
    {
        var resolution = new ResolutionContext();

        var playerCard = game.PlayerCurrentCard;
        var enemyCard = game.EnemyCurrentCard;

        resolution.Get(playerCard);
        resolution.Get(enemyCard);

        if (result.Outcome is ConflictOutcome.EnemyWin)
            resolution.Get(playerCard).WillBeLost = true;

        RunPostEffects(playerCard, enemyCard, resolution);
        RunPostEffects(enemyCard, playerCard, resolution);

        CombatEventManager.ResolveCardsVisual(game, resolution, result.Outcome);

        switch (result.Outcome)
        {
            case ConflictOutcome.PlayerWin: ResolvePlayerWin(game, resolution); break;
            case ConflictOutcome.EnemyWin: ResolveEnemyWin(game, resolution); break;
            case ConflictOutcome.Tie: ResolveTie(game, resolution); break;
        }

        Cleanup(game, resolution);
    }

    static void RunPostEffects(Card source, Card other, ResolutionContext ctx)
    {
        foreach (var sticker in source.Stickers.Select(s => s.Logic))
            sticker.AfterResolution(ctx, source, other);
    }

    static void ResolvePlayerWin(GameContext game, ResolutionContext resolution)
    {
        game.Enemy.Damage();
        CombatEventManager.PlayDialogue(game.Enemy.Data.dialogue.loseRound.PickOne(), game.Enemy.Data.Color);
    }

    static void ResolveEnemyWin(GameContext game, ResolutionContext resolution)
    {
        CombatEventManager.PlayDialogue(game.Enemy.Data.dialogue.winRound.PickOne(), game.Enemy.Data.Color);
    }

    static void ResolveTie(GameContext game, ResolutionContext resolution)
    {
    }

    static void Cleanup(GameContext context, ResolutionContext resolution)
    {
        context.PlayerCurrentCard = null;
        context.EnemyCurrentCard = null;
        context.AvailableStickers.Clear();
    }
}