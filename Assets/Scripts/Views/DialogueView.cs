using System.Collections;
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

        void ShowDialogue(string line, Color backColor, Color textColor, float display = -1f)
        {
            if (currentDialogue != null)
                StopCoroutine(currentDialogue);

            dialogueBox.gameObject.SetActive(true);

            for (var i = 0; i < backgroundImages.Length; i++)
                backgroundImages[i].color = backColor;
            tmpBody.color = textColor;

            animator.clip = animator.GetClip("animDialogueBoxPopUp");
            animator.Play();

            currentDialogue = StartCoroutine(TypeDialogue(line, display > 0 ? display : displayTime));
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

        IEnumerator TypeDialogue(string line,  float display)
        {
            tmpBody.text = "";
            foreach (var c in line)
            {
                tmpBody.text += c;
                yield return new WaitForSeconds(characterDelay);
            }

            yield return new WaitForSeconds(display);

            HideDialogue();
        }
    }
}