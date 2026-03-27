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
        
        CardCombatView playerCombatView;
        CardCombatView enemyCombatView;

        public CombatResolutionService()
        {
            CombatEventManager.OnEnemyEvaluationReady += SetEnemyView;
            CombatEventManager.OnPlayerEvaluationReady += SetPlayerView;
        }

        void SetPlayerView(EvaluationView v, CardCombatView c)
        {
            playerView = v;
            playerCombatView = c;
        }

        void SetEnemyView(EvaluationView v, CardCombatView c)
        {
            enemyView = v;
            enemyCombatView = c;
        }

        public IEnumerator Resolve(GameContext context)
        {
            var result = ConflictResolver.Resolve(context);

            yield return PlayEvaluation(result);
            yield return new WaitForSeconds(2);
            ConflictResolver.ApplyOutcome(result, context);

            //TODO: check if this is needed
            yield return new WaitForSeconds(2);
        }

        IEnumerator PlayEvaluation(ConflictResult result)
        {
            yield return DOTween.Sequence()
                                .Join(playerView.Play(result.PlayerEvaluation, playerCombatView))
                                .Join(enemyView.Play(result.EnemyEvaluation, enemyCombatView))
                                .WaitForCompletion();
        }
    }
}