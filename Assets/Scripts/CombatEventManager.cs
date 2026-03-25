using System;
using System.Collections.Generic;
using Data;
using Data.Stickers;
using UnityEngine;
using Views;

public static class CombatEventManager
{
        public static event Action<CardView> OnPlayerPlaysCard;
        public static event Action<CardView> OnPlayCard;
        public static event Action<Card> OnEnemyPlayCard;
        
        public static Action<Card, StickerPlacement> OnEnemyPlaceStickerPreview;
        public static event Action<StickerPlacement, Card> OnAddSticker;
        public static event Action OnClearTable;
        public static event Action<int, int> OnEnemyHealthChanged;
        public static event Action<EvaluationView> OnEnemyEvaluationReady;
        public static event Action<EvaluationView> OnPlayerEvaluationReady;
        public static event Action<Enemy> OnEnemySet;
        public static event Action<List<StickerInstance>> OnRevealStickers;
        public static event Action OnClearStickers;
        public static event Action<string> OnPlayDialogue;
        
        public static event Action<StickerData, Vector3, Quaternion> OnStickerHoverEnter;
        public static event Action OnStickerHoverExit;
        
        public static void PlayCard(CardView view) => OnPlayCard?.Invoke(view);
        public static void EnemyPlayCard(Card card) => OnEnemyPlayCard?.Invoke(card);
        public static void AddSticker(StickerPlacement sticker, Card card) => OnAddSticker?.Invoke(sticker, card);
        public static void ClearTable() => OnClearTable?.Invoke();

        public static void EnemyEvaluationReady(EvaluationView evaluationView) => 
                OnEnemyEvaluationReady?.Invoke(evaluationView);
        public static void PlayerEvaluationReady(EvaluationView evaluationView) => 
                OnPlayerEvaluationReady?.Invoke(evaluationView);

        public static void PlayerPlaysCard(CardView view) => OnPlayerPlaysCard?.Invoke(view);
        public static void EnemyHealthChanged(int current, int max) => OnEnemyHealthChanged?.Invoke(current, max);
        public static void SetEnemy(Enemy enemy) => OnEnemySet?.Invoke(enemy);

        public static void RevealStickers(List<StickerInstance> stickers) => OnRevealStickers?.Invoke(stickers);
        public static void ClearStickers() => OnClearStickers?.Invoke();

        public static void PlayDialogue(string message) => OnPlayDialogue?.Invoke(message);

        public static void StickerHoverEnter(StickerData data, Vector3 pos, Quaternion rot) => 
                OnStickerHoverEnter?.Invoke(data, pos, rot);
        public static void StickerHoverExit() => OnStickerHoverExit?.Invoke();
}