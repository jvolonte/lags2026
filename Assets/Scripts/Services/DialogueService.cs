using System;
using Audio;
using Data;
using Presenters;
using Utils;

namespace Services
{
    public static class DialogueService
    {
        public static void LoseGameDialogue(EnemyData data) =>
            PlayDialogue(data, data.dialogue.onLoseGame);

        public static void ThinkingDialogue(EnemyData data) =>
            PlayDialogue(data, data.dialogue.thinking.PickOne());
        
        public static void WinRoundDialogue(EnemyData data) =>
            PlayDialogue(data, data.dialogue.winRound.PickOne());
        
        public static void LoseRoundDialogue(EnemyData data) =>
            PlayDialogue(data, data.dialogue.loseRound.PickOne());

        public static void TutorialEncounterDialogue(EnemyData data) =>
            PlayDialogue(data, data.dialogue.encounterPhase, true);

        public static void TutorialStickerDialogue(EnemyData data) =>
            PlayDialogue(data, data.dialogue.stickerPhase, true);

        public static void GameStart(EnemyData data, bool isTutorial = false) =>
            PlayDialogue(data, data.dialogue.onGameStart, isTutorial);

        static void PlayDialogue(EnemyData data, string message, bool tutorial = false)
        {
            var pitch = UnityEngine.Random.Range(0.9f, 1.2f);
            SfxManager.Play(FindDialogueSfxId(data), pitch: pitch);
            CombatEventManager.PlayDialogue(message, data.backgroundColor, data.textColor, tutorial);
        }

        static SfxClipId FindDialogueSfxId(EnemyData data) =>
            data.id switch
            {
                EnemyId.Alfonso => SfxClipId.AlfonsoVoice,
                EnemyId.Carmen => SfxClipId.CarmenVoice,
                EnemyId.Vincent => SfxClipId.VincentVoice,
                EnemyId.Ivan => SfxClipId.IvanVoice,
                EnemyId.Baltasar => SfxClipId.BaltasarVoice,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}