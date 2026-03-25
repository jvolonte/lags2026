using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Views
{
    public class ButtonsView : MonoBehaviour
    {
        [SerializeField] Button addRandomStickerButton;
        [SerializeField] Button nextEnemyButton;

        [SerializeField] GameStateManager gameStateManager;

        void Start()
        {
            var context = gameStateManager.Context;

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