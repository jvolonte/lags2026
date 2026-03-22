namespace Factories
{
    public class CardFactory
    {
        public Card CreateRandom() =>
            new(
                UnityEngine.Random.Range(1, 13),
                (Suit)UnityEngine.Random.Range(0, 4)
            );
    }
}