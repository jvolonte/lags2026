using System;
using Stickers;
using Views;

public static class CombatEventManager
{
        public static event Action<Card> OnPlayerPlaysCard;
        
        public static event Action<Card> OnPlayCard;
        public static event Action<Card> OnEnemyPlayCard;
        public static event Action<ISticker, Card> OnAddSticker;
        public static event Action OnClearTable;
        
        public static event Action<EvaluationView> OnEnemyEvaluationReady;
        public static event Action<EvaluationView> OnPlayerEvaluationReady;
        
        public static void PlayCard(Card card) => OnPlayCard?.Invoke(card);
        public static void EnemyPlayCard(Card card) => OnEnemyPlayCard?.Invoke(card);
        public static void AddSticker(ISticker sticker, Card card) => OnAddSticker?.Invoke(sticker, card);
        public static void ClearTable() => OnClearTable?.Invoke();

        public static void EnemyEvaluationReady(EvaluationView evaluationView) => 
                OnEnemyEvaluationReady?.Invoke(evaluationView);
        public static void PlayerEvaluationReady(EvaluationView evaluationView) => 
                OnPlayerEvaluationReady?.Invoke(evaluationView);

        public static void PlayerPlaysCard(Card card) => OnPlayerPlaysCard?.Invoke(card);
}