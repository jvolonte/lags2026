namespace CardZones
{
    public class DiscardPile : CardZone
    {
        public override Card Peek() => Cards.Count > 0 ? Cards[^1] : null;
    }
}