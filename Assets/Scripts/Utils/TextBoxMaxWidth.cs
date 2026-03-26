using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Clamps a TextMeshProUGUI element's preferred width to a maximum value.
/// When the text changes, it recalculates the preferred width, applies the cap
/// via the LayoutElement, and forces a layout rebuild on the parent so that
/// Content Size Fitter + Vertical Layout Group pick up the new size.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
[RequireComponent(typeof(LayoutElement))]
public class TextBoxMaxWidth : MonoBehaviour
{
    [Tooltip("Maximum allowed width for the text box (in pixels).")]
    [SerializeField] private float maxWidth = 400f;

    private TMP_Text _tmpText;
    private LayoutElement _layoutElement;
    private RectTransform _parentRect;
    private bool _isRefreshing;

    private void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();
        _layoutElement = GetComponent<LayoutElement>();
        _parentRect = transform.parent as RectTransform;
    }

    private void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        Refresh();
    }

    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    private void OnTextChanged(Object obj)
    {
        if (obj == _tmpText)
        {
            Refresh();
        }
    }

    /// <summary>
    /// Call this manually if you change maxWidth at runtime.
    /// </summary>
    public void Refresh()
    {
        if (_isRefreshing) return;
        _isRefreshing = true;

        try
        {
            _tmpText.ForceMeshUpdate();

            float preferred = _tmpText.GetPreferredValues().x;

            if (preferred > maxWidth)
            {
                _layoutElement.preferredWidth = maxWidth;
                float wrappedHeight = _tmpText.GetPreferredValues(maxWidth, 0f).y;
                _layoutElement.preferredHeight = wrappedHeight;
            }
            else
            {
                _layoutElement.preferredWidth = preferred;
                float naturalHeight = _tmpText.GetPreferredValues(preferred, 0f).y;
                _layoutElement.preferredHeight = naturalHeight;
            }

            if (_parentRect != null)
            {
                LayoutRebuilder.MarkLayoutForRebuild(_parentRect);
            }
        }
        finally
        {
            _isRefreshing = false;
        }
    }
}