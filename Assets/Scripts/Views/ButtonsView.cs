using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Views
{
    public class ButtonsView : MonoBehaviour
    {
        [SerializeField] Button playRandomCardButton;
        [SerializeField] Button addRandomStickerButton;
        [SerializeField] Button nextEnemyButton;

        [SerializeField] GameStateManager gameStateManager;

        void Start()
        {
            var context = gameStateManager.Context;

            playRandomCardButton.onClick.AddListener(() =>
            {
                if (gameStateManager.CurrentState != GameState.PlayerPlaysCard)
                {
                    Debug.Log("It's not the time to play a card!");
                    return;
                }
                
                var player = context.Player;
                var hand = player.Hand;
                var card = hand.Cards.PickOne();
                player.Play(card);
            });

            addRandomStickerButton.onClick.AddListener(() =>
            {
                if (gameStateManager.CurrentState != GameState.PlayerPlaceSticker)
                {
                    Debug.Log("It's not the time to place a sticker!");
                    return;
                }
                    
                var sticker = context.AvailableStickers.PickOne();
                var cards = context.Player.Hand.Cards.Append(context.PlayerCurrentCard);
                var placement = new StickerPlacement()
                {
                    Logic = sticker.Logic,
                    Data = sticker.Data,
                    LocalPosition = new Vector2()
                };
                CombatEventManager.AddSticker(placement, cards.PickOne());
            });
            
            nextEnemyButton.onClick.AddListener(() => gameStateManager.Debug_KillEnemyAndAdvance());
        }
    }
}