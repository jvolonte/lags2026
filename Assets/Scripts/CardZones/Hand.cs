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

        public void Play(Card card)
        {
            if (Cards.Contains(card))
                Cards.Remove(card);
        }
    }
}