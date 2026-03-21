using UnityEngine;

namespace Stickers
{
    public class MultiplierSticker : ISticker
    {
        public int Priority => 100;

        public float Value;

        public MultiplierSticker(float value) => Value = value;

        public void Resolve(Card source, Card other) =>
            source.Evaluation = Mathf.FloorToInt(source.Evaluation * Value);
        
        public override string ToString() => $"Multiplier: {Value}";
    }
}