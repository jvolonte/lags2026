using System.Collections;
using DG.Tweening;
using UnityEngine;
using Views;

namespace Services
{
    public class CombatResolutionService
    {
        EvaluationView playerView;
        EvaluationView enemyView;

        public CombatResolutionService()
        {
            CombatEventManager.OnEnemyEvaluationReady += SetEnemyView;
            CombatEventManager.OnPlayerEvaluationReady += SetPlayerView;
        }

        void SetPlayerView(EvaluationView v) => playerView = v;
        void SetEnemyView(EvaluationView v) => enemyView = v;

        public IEnumerator Resolve(GameContext context)
        {
            var result = ConflictResolver.Resolve(context);

            yield return PlayEvaluation(context);
            yield return new WaitForSeconds(2);
            ConflictResolver.ApplyOutcome(result, context);
            
            //TODO: check if this is needed
            yield return new WaitForSeconds(2);
        }

        IEnumerator PlayEvaluation(GameContext context)
        {
            var playerEval = context.PlayerCurrentCard.Calculate(context.EnemyCurrentCard, context);
            var enemyEval = context.EnemyCurrentCard.Calculate(context.PlayerCurrentCard, context);

            yield return DOTween.Sequence()
                                .Join(playerView.Play(playerEval))
                                .Join(enemyView.Play(enemyEval))
                                .WaitForCompletion();
        }
    }
}