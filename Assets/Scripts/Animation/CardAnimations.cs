using System.Collections;
using UnityEngine;

public class CardAnimations : MonoBehaviour
{
    public MeshRenderer meshCard;
    public SkinnedMeshRenderer meshSelection;

    [Header("Animation Parameters")]
    public float reflectionDuration;
    public float highlightOnDuration;
    public float highlightOffDuration;
    public AnimationCurve curveReflection;
    public AnimationCurve curveSelection;

    private bool highlighted;
    private float highlightedTime;

    private Coroutine coroutineReflection;

    private void OnEnable()
    {
        meshCard.material.SetFloat("_ReflectionValue", 0);
        meshSelection.SetBlendShapeWeight(0, 0);
        meshSelection.gameObject.SetActive(false);

        highlighted = false;
        highlightedTime = 0f;
        coroutineReflection = null;
    }
    private void LateUpdate()
    {
        if (highlighted)
            highlightedTime = Mathf.Clamp01(highlightedTime + Time.deltaTime / highlightOnDuration);
        else
            highlightedTime = Mathf.Clamp01(highlightedTime - Time.deltaTime / highlightOffDuration);

        if (highlightedTime > 0f) 
        {
            meshSelection.gameObject.SetActive(true);
            meshSelection.SetBlendShapeWeight(0, curveSelection.Evaluate(highlightedTime) * 100f);
        }
        else
        {
            meshSelection.gameObject.SetActive(false);
        }
                
    }
    public void Highlight (bool on)
    {
        if (highlighted == on) { return; }

        highlighted = on;

        if (highlighted && coroutineReflection == null)
            coroutineReflection = StartCoroutine(PlayReflection(reflectionDuration));
    }
    private IEnumerator PlayReflection (float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t = Mathf.Clamp01(t + Time.deltaTime / duration);
            meshCard.material.SetFloat("_ReflectionValue", 1-curveReflection.Evaluate(t));
            yield return null;
        }

        coroutineReflection = null;
    }
}
