using System.Collections;
using TMPro;
using UnityEngine;

namespace Views
{
    public class DialogueView : MonoBehaviour
    {
        [SerializeField] TextMeshPro textMesh;
        [SerializeField] float characterDelay = 0.05f;
        [SerializeField] float displayTime = 2f;

        Coroutine currentDialogue;

        void Awake()
        {
            CombatEventManager.OnPlayDialogue += ShowDialogue;
        }

        void OnDestroy()
        {
            CombatEventManager.OnPlayDialogue -= ShowDialogue;
        }

        public void ShowDialogue(string line)
        {
            if (currentDialogue != null)
                StopCoroutine(currentDialogue);

            currentDialogue = StartCoroutine(TypeDialogue(line));
        }

        IEnumerator TypeDialogue(string line)
        {
            textMesh.text = "";
            foreach (var c in line)
            {
                textMesh.text += c;
                yield return new WaitForSeconds(characterDelay);
            }

            yield return new WaitForSeconds(displayTime);
            textMesh.text = "";
            currentDialogue = null;
        }
    }
}