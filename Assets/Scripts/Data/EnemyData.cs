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
        public int stickersInCards = 0;
        
        [Header("Presentation")]
        public GameObject prefab;
    }
}