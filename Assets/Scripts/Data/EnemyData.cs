using Audio;
using Presenters;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Game/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public EnemyId id;
        public DialogueData dialogue;

        [Header("Stats")]
        public int health = 3;
        public Vector2Int stickersInCards;
        
        [Header("Presentation")]
        public GameObject prefab;
        public BgmClipId bgmClipId;
        public TimeOfDay TimeOfDay;

        [Header("Texts Presentation")]
        public Color backgroundColor;
        public Color textColor;

    }
}

public enum TimeOfDay
{
    Day,
    Night
}