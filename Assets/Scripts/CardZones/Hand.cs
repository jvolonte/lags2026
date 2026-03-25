using Views;

namespace CardZones
{
    public class Hand : CardZone
    {
        public int MaxSize = 3;

        public bool CanDraw => Cards.Count < MaxSize;

        public override void Add(Card card)
        {
            if (Cards.Count >= MaxSize) return;
            base.Add(card);
        }

        public void Play(CardView view)
        {
            var card = view.GetCard();
            
            if (Cards.Contains(card))
            {
                Cards.Remove(card);
                TriggerCardRemoved(card);
            }
            
            CombatEventManager.PlayCard(view);
        }
    }
}