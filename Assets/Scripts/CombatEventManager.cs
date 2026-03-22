using System;
using Stickers;

public static class CombatEventManager
{
        public static event Action<Card> OnPlayCard;
        public static event Action<Card> OnEnemyPlayCard;
        public static event Action<ISticker, Card> OnAddSticker;
        public static event Action OnClearTable;
        
        public static void PlayCard(Card card) => OnPlayCard?.Invoke(card);
        public static void EnemyPlayCard(Card card) => OnEnemyPlayCard?.Invoke(card);
        public static void AddSticker(ISticker sticker, Card card) => OnAddSticker?.Invoke(sticker, card);
        public static void ClearTable() => OnClearTable?.Invoke();
}