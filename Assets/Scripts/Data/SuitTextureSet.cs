using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class SuitTextureSet
    {
        public Suit suit;
        public Texture2D defaultTexture;
        public List<CardValueTexture> values;
    }
}