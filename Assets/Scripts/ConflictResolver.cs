using System.Linq;
using UnityEngine;
using Utils;

public class ConflictResolver
{
    public ConflictResult Resolve(GameContext context)
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
            Outcome = outcome
        };
    }

    public void ApplyOutcome(ConflictResult result, GameContext game)
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

        switch (result.Outcome)
        {
            case ConflictOutcome.PlayerWin: ResolvePlayerWin(game, resolution); break;
            case ConflictOutcome.EnemyWin: ResolveEnemyWin(game, resolution); break;
            case ConflictOutcome.Tie: ResolveTie(game, resolution); break;
        }
    }

    void RunPostEffects(Card source, Card other, ResolutionContext ctx)
    {
        foreach (var sticker in source.Stickers.Select(s => s.Logic))
            sticker.AfterResolution(ctx, source, other);
    }

    void ResolvePlayerWin(GameContext game, ResolutionContext resolution)
    {
        game.Enemy.Damage();
        CombatEventManager.PlayDialogue(game.Enemy.Data.dialogue.loseRound.PickOne());

        HandleCardAfterCombat(game.PlayerCurrentCard, game, resolution);
        HandleCardAfterCombat(game.EnemyCurrentCard, game, resolution);

        Cleanup(game);
    }

    void HandleCardAfterCombat(Card card, GameContext context, ResolutionContext resolution)
    {
        var outcome = resolution.Get(card);

        if (outcome.Destroy)
            return;

        if (outcome.WillBeLost)
            return;

        context.Player.Discard.Add(card);
    }

    void ResolveEnemyWin(GameContext game, ResolutionContext resolution)
    {
        Debug.Log("---ENEMY WON---");
        CombatEventManager.PlayDialogue(game.Enemy.Data.dialogue.winRound.PickOne());

        HandleCardAfterCombat(game.PlayerCurrentCard, game, resolution);
        Cleanup(game);
    }

    void ResolveTie(GameContext game, ResolutionContext resolution)
    {
        Debug.Log("---TIE---");
        HandleCardAfterCombat(game.PlayerCurrentCard, game, resolution);

        Cleanup(game);
    }

    void Cleanup(GameContext context)
    {
        context.PlayerCurrentCard = null;
        context.EnemyCurrentCard = null;
        context.AvailableStickers.Clear();
        CombatEventManager.ClearTable();
    }
}