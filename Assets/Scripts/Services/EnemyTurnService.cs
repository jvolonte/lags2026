using System.Collections;
using System.Linq;
using Data;
using Presenters;
using UnityEngine;

namespace Services
{
    public class EnemyTurnService
    {
        readonly EnemyCardPresenter presenter;

        public EnemyTurnService(EnemyCardPresenter presenter)
        {
            this.presenter = presenter;
        }

        public IEnumerator PlaceSticker(GameContext context)
        {
            
            yield return new WaitForSeconds(2);
            
            var enemyCard = context.EnemyCurrentCard;
            var best = StickerEvaluationService.PickBestSticker(
                enemyCard,
                context.PlayerCurrentCard,
                context.AvailableStickers.Select(s => s.Data).ToList()
            );

            var sticker = context.AvailableStickers.First(s => s.Data == best);

            var placement = new StickerPlacement
            {
                Logic = sticker.Logic,
                Data = sticker.Data,
                LocalPosition = presenter.GetRandomStickerPosition()
            };

            enemyCard.Stickers.Add(placement);
            CombatEventManager.OnEnemyPlaceStickerPreview?.Invoke(enemyCard, placement);
        }
    }
}