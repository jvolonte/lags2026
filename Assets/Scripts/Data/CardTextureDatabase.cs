using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Cards/Card Texture Database")]
    public class CardTextureDatabase : ScriptableObject
    {
        public Texture2D globalDefault;

        public List<SuitTextureSet> suits;

        public Texture2D GetTexture(Suit suit, int value)
        {
            var suitSet = suits.Find(s => s.suit == suit);

            if (suitSet != null)
            {
                var specific = suitSet.values
                                      .Find(v => v.value == value);

                if (specific != null && specific.texture != null)
                    return specific.texture;

                if (suitSet.defaultTexture != null)
                    return suitSet.defaultTexture;
            }

            return globalDefault;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            foreach (var suit in suits)
            {
                var values = new HashSet<int>();
                foreach (var v in suit.values.Where(v => !values.Add(v.value))) 
                    Debug.LogWarning($"Duplicate value {v.value} in {suit.suit}");
            }
        }
#endif
    }
}