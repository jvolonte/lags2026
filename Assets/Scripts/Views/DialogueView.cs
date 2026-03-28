using System.Collections;
using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class DialogueView : MonoBehaviour
    {
        [SerializeField] GameObject dialogueBox;
        [SerializeField] TextMeshProUGUI tmpBody;
        [SerializeField] Image[] backgroundImages;
        [SerializeField] UnityEngine.Animation animator;
        [SerializeField] Color defaultBackgroundColor;
        [SerializeField] Color defaultTextColor;
        [SerializeField] float characterDelay = 0.05f;
        [SerializeField] float displayTime = 2f;

        Coroutine currentDialogue;
        Coroutine hideDialogue;

        bool currentDialogueIsTutorial;

        void Awake()
        {
            CombatEventManager.OnPlayDialogue += ShowDialogue;

            animator["animDialogueBoxPopUp"].wrapMode = WrapMode.Once;
            animator["animDialogueBoxLeave"].wrapMode = WrapMode.Once;
        }

        void OnDestroy()
        {
            CombatEventManager.OnPlayDialogue -= ShowDialogue;
        }

        void OnEnable()
        {
            StopAllCoroutines();
            dialogueBox.SetActive(false);
            currentDialogue = null;
            hideDialogue = null;
        }

        void ShowDialogue(string line, Color backColor, Color textColor, bool tutorial)
        {
            currentDialogueIsTutorial = tutorial;

            if (currentDialogue != null)
                StopCoroutine(currentDialogue);

            dialogueBox.gameObject.SetActive(true);

            for (var i = 0; i < backgroundImages.Length; i++)
                backgroundImages[i].color = backColor;
            tmpBody.color = textColor;

            animator.clip = animator.GetClip("animDialogueBoxPopUp");
            animator.Play();

            currentDialogue = StartCoroutine(TypeDialogue(line, displayTime));
        }

        void HideDialogue()
        {
            if (currentDialogue == null) return;
            if (hideDialogue != null) return;

            animator.clip = animator.GetClip("animDialogueBoxLeave");
            animator.Play();

            hideDialogue = StartCoroutine(HideDialogueWait());

            currentDialogue = null;
        }

        IEnumerator HideDialogueWait()
        {
            yield return null;

            while (animator.isPlaying) yield return null;

            dialogueBox.gameObject.SetActive(false);

            hideDialogue = null;
        }

        IEnumerator TypeDialogue(string line, float display)
        {
            tmpBody.text = "";

            var counter = 0;
            var charsPerSound = 2;

            var lastSoundTime = 0f;
            var minInterval = 0.04f;

            foreach (var c in line)
            {
                tmpBody.text += c;

                if (char.IsLetterOrDigit(c))
                {
                    counter++;

                    if (counter >= charsPerSound && Time.time - lastSoundTime > minInterval)
                    {
                        counter = 0;
                        lastSoundTime = Time.time;

                        SfxManager.Play(SfxClipId.Dialogue, 1f, Random.Range(0.92f, 1.08f));
                    }
                }

                yield return new WaitForSeconds(characterDelay);
            }

            if (!currentDialogueIsTutorial)
            {
                yield return new WaitForSeconds(display);
                HideDialogue();
            }
        }
    }
}