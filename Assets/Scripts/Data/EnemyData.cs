using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Game/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public string id;
        public DialogueData dialogue;

        [Header("Stats")]
        public int health = 3;
        public Vector2Int stickersInCards;
        
        [Header("Presentation")]
        public GameObject prefab;

        [Header("Texts Presentation")]
        public Color backgroundColor;
        public Color textColor;
    }
}