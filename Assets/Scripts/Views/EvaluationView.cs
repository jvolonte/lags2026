using TMPro;
using UnityEngine;

namespace Views
{
    public class EvaluationView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;

        public void SetValue(int value)
        {
            text.text = value.ToString();
        }
    }
}