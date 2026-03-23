using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Game/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public string id;

        [Header("Stats")]
        public int health = 3;
        
        [Header("Presentation")]
        public GameObject prefab;
    }
}