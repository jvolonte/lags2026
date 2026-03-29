using System;
using System.Collections;
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

            if (context.IsTutorialRound)
            {
                PlayTutorialDialogue(context, result);
                InputController.Instance.ConsumeContinue();
                yield return new WaitUntil(() => InputController.Instance.ConsumeContinue());
            }
            else
                yield return new WaitForSeconds(2);

            ConflictResolver.ApplyOutcome(result, context);

            yield return new WaitForSeconds(2);
        }

        static void PlayTutorialDialogue(GameContext context, ConflictResult result)
        {
            switch (result.Outcome)
            {
                case ConflictOutcome.EnemyWin:
                    DialogueService.TutorialLoseDialogue(context.Enemy.Data);
                    break;
                case ConflictOutcome.PlayerWin:
                    DialogueService.TutorialWinDialogue(context.Enemy.Data);
                    break;
                case ConflictOutcome.Tie:
                    DialogueService.TutorialTieDialogue(context.Enemy.Data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        IEnumerator PlayEvaluation(ConflictResult result)
        {
            yield return enemyView.Play(result.EnemyEvaluation, enemyCombatView);
            yield return playerView.Play(result.PlayerEvaluation, playerCombatView);
        }
    }
}