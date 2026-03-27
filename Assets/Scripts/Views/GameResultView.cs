using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Views
{
    public class GameResultView : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] TextMeshProUGUI messageText;
        [SerializeField] Button playAgainButton;
        [SerializeField] CanvasGroup canvasGroup;

        [SerializeField] TextMeshProUGUI thanks;

        [Header("Configuration")]
        [SerializeField]
        float typeSpeed = 1f;

        [SerializeField] float linePause = 0.5f;
        [SerializeField] float buttonDelay = 1f;

        void Awake()
        {
            root.SetActive(false);
            playAgainButton.onClick.AddListener(Restart);
        }

        public void ShowWin()
        {
            var message = "No one else remains.\n" +
                          "The bar is quiet.\n" +
                          "You've won.\n" +
                          "\n" +
                          "The door opens.\n" +
                          "A new face takes the empty seat.";

            StopAllCoroutines();
            StartCoroutine(ShowRoutine(message));
        }

        IEnumerator ShowRoutine(string fullText)
        {
            Show();
            messageText.text = "";
            playAgainButton.gameObject.SetActive(false);
            thanks.DOFade(0, 0);

            var lines = fullText.Split('\n');

            yield return new WaitForSeconds(0.25f);
            
            for (var i = 0; i < lines.Length; i++)
            {
                yield return StartCoroutine(TypeLine(lines[i]));

                if (i < lines.Length - 1)
                    yield return Wait(linePause);
            }

            yield return Wait(buttonDelay);
            ShowButton();
            yield return Wait(buttonDelay);
            ShowThanks();
        }

        void ShowThanks()
        {
            thanks.DOFade(1f, 0.3f).SetEase(Ease.OutBack);
        }

        void ShowButton()
        {
            var t = playAgainButton.transform;

            playAgainButton.gameObject.SetActive(true);

            t.localScale = Vector3.zero;

            t.DOScale(1f, 0.4f)
             .SetEase(Ease.OutBack);
        }

        IEnumerator TypeLine(string line)
        {
            foreach (var t in line)
            {
                messageText.text += t;
                yield return Wait(typeSpeed);
            }

            messageText.text += "\n";
        }

        static WaitForSecondsRealtime Wait(float time) => new(time);

        public void ShowLose()
        {
            var message = "You Lose\n" +
                          "A quiet nod from across the table.\n" +
                          "The cards turn cold.\n" +
                          "The table claims its due.";

            StopAllCoroutines();
            StartCoroutine(ShowRoutine(message));
        }

        void Show()
        {
            root.SetActive(true);

            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.3f);

            root.transform.localScale = Vector3.one * 0.8f;
            root.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }

        void Restart() => 
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}