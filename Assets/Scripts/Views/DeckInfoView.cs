using TMPro;
using UnityEngine;
using Views;

public class DeckInfoView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDeckAmount;
    [SerializeField] TextMeshProUGUI textDiscardAmount;

    private void Start()
    {
        DiscardPileView.OnDiscardChange += RefreshDiscard;
        DeckView.OnDeckChange += RefreshDeck;
    }

    public void RefreshDiscard (int newValue)
    {
        textDiscardAmount.text = newValue.ToString();
    }
    public void RefreshDeck (int newValue)
    {
        textDeckAmount.text = newValue.ToString();
    }
}
