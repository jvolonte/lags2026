using Data;
using Utils;

namespace Services
{
    public static class DialogueService
    {
        public static void LoseGameDialogue(EnemyData data) =>
            CombatEventManager.PlayDialogue(data.dialogue.onLoseGame, data.backgroundColor, data.textColor);

        public static void ThinkingDialogue(EnemyData data) =>
            CombatEventManager.PlayDialogue(data.dialogue.thinking.PickOne(), data.backgroundColor, data.textColor);

        public static void TutorialEncounterDialogue(EnemyData data) =>
            CombatEventManager.PlayDialogue(data.dialogue.encounterPhase, data.backgroundColor, data.textColor);

        public static void TutorialStickerDialogue(EnemyData data) =>
            CombatEventManager.PlayDialogue(data.dialogue.stickerPhase, data.backgroundColor, data.textColor);

        public static void GameStart(EnemyData data) =>
            CombatEventManager.PlayDialogue(data.dialogue.onGameStart, data.backgroundColor, data.textColor);
    }
}