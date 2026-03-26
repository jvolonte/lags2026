using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CardAnimations : MonoBehaviour
{
    private const string ANIM_CLIP_BURN = "animCardBurn";

    public MeshRenderer meshCard;
    public SkinnedMeshRenderer meshSelection;
    public GameObject shadowReciever;
    public UnityEngine.Animation animator;
    public GameObject shadowCaster;
    public ParticleSystem vfxBurningCard;

    [Header("Animation Parameters")]
    public float reflectionDuration;
    public float highlightOnDuration;
    public float highlightOffDuration;
    public AnimationCurve curveReflection;
    public AnimationCurve curveSelection;
    public Color colorHighlighted;

    private bool burning;
    private bool highlighted;
    private float highlightedTime;

    private Coroutine coroutineBurning;
    private Coroutine coroutineReflection;

    private System.Action onBurnEndAction;

    private void Awake()
    {
        animator[ANIM_CLIP_BURN].wrapMode = WrapMode.Once;
    }
    private void OnEnable()
    {
        meshCard.material.SetFloat("_ReflectionValue", 0);
        meshSelection.SetBlendShapeWeight(0, 0);
        meshSelection.gameObject.SetActive(false);
        meshSelection.material.SetColor("_OverrideColor", Color.black);
        shadowCaster.gameObject.SetActive(true);
        vfxBurningCard.Stop(true);

        burning = false;
        highlighted = false;
        highlightedTime = 0f;
        coroutineReflection = null;
        coroutineBurning = null;
        onBurnEndAction = null;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
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
            meshSelection.material.SetColor("_OverrideColor", colorHighlighted);
        }
        else
        {
            meshSelection.gameObject.SetActive(false);
        }
                
    }
    public void Burn (System.Action onEnd = null)
    {
        animator.clip = animator.GetClip(ANIM_CLIP_BURN);
        animator.Play();

        coroutineBurning = StartCoroutine(Burning(onEnd));
    }
    IEnumerator Burning(System.Action onEnd)
    {
        float watchdog = 0f;

        onBurnEndAction = onEnd;

        burning = true;
        shadowCaster.gameObject.SetActive(false);

        yield return null;

        while (animator.isPlaying && watchdog < 3f) 
        { 
            watchdog += Time.deltaTime; 
            yield return null;  
        }

        burning = false;

        onBurnEndAction?.Invoke();

        onBurnEndAction = null;
        coroutineBurning = null;      
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
