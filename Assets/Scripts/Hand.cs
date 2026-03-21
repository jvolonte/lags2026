public class Hand : CardZone
{
    public int MaxSize = 3;

    public bool CanDraw => cards.Count < MaxSize;

    public override void Add(Card card)
    {
        if (cards.Count >= MaxSize) return;
        base.Add(card);
    }
}