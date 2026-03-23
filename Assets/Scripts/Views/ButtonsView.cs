using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class ButtonsView : MonoBehaviour
    {
        [SerializeField] Button playRandomCardButton;
        [SerializeField] Button addRandomStickerButton;

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
                CombatEventManager.AddSticker(sticker.Logic, cards.PickOne());
            });
        }
    }
}