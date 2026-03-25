using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue")]
    public class DialogueData : ScriptableObject
    {
        [Header("Game Start")]
        public string onGameStart;
        
        [Header("Thinking")]
        public string[] thinking;
        
        [Header("Round")]
        public string[] winRound;
        public string[] loseRound;

        [Header("Game")]
        public string onLoseGame;

        [Header("Tutorial")] 
        [TextArea] public string stickerPhase;
        [TextArea] public string encounterPhase;
    }
}