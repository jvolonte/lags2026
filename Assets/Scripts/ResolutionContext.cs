using System.Collections.Generic;

public class ResolutionContext
{
    public Dictionary<Card, CardOutcome> Outcomes = new();

    public CardOutcome Get(Card card)
    {
        if (!Outcomes.TryGetValue(card, out var outcome))
        {
            outcome = new CardOutcome();
            Outcomes[card] = outcome;
        }

        return outcome;
    }
}

public class CardOutcome
{
    public bool Destroy;
}