using System.Collections;
using System.Linq;
using Audio;
using Data.Stickers;
using Stickers;
using UnityEditor;
using UnityEngine;

namespace Views
{
    public class StickerView : MonoBehaviour
    {
        private const string ANIMATION_TRIGGER = "animStickerTrigger";
        private static readonly Vector3 highlightDisplacement = new Vector3(0f, 0.1f, -0.1f);

        [SerializeField] UnityEngine.Animation animator;
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] Transform root;

        [Header("Burn Animation")]
        [SerializeField] Texture burnNoise;
        [SerializeField] Color burnEdgeColor;
        [SerializeField] Color burnColor;

        [Header("Animation Parameters")]
        public float reflectionDuration;
        public float highlightOnDuration;
        public float highlightOffDuration;
        public AnimationCurve curveReflection;
        public AnimationCurve curveSelection;

        StickerInstance instance;
        public MeshRenderer MeshRenderer => meshRenderer;

        public bool Interactable { get; set; } = false;
        public bool Dragging { get; set; } = false;
        public bool CanDrag { get; private set; } = true;

        private float randomAngle;
        private bool highlighted;
        private float highlightedTime;

        private Coroutine coroutineReflection;

        private void Awake()
        {
            meshRenderer.material.SetTexture("_BurnNoise", burnNoise);
            meshRenderer.material.SetColor("_Burn", burnEdgeColor);
            animator[ANIMATION_TRIGGER].wrapMode = WrapMode.Once;
            randomAngle = Mathf.Sign(Random.Range(-1f, 1f)) * Random.Range(10f, 15f);
        }

        private void LateUpdate()
        {
            if (!Interactable) return;

            if (highlighted)
                highlightedTime = Mathf.Clamp01(highlightedTime + Time.deltaTime / highlightOnDuration);
            else
                highlightedTime = Mathf.Clamp01(highlightedTime - Time.deltaTime / highlightOffDuration);

            if (highlightedTime > 0f)
            {
                root.localPosition = highlightDisplacement * curveSelection.Evaluate(highlightedTime);
                root.localRotation = Quaternion.AngleAxis(randomAngle * curveSelection.Evaluate(highlightedTime), Vector3.forward);
            }
            else
            {
                root.localPosition = Vector3.zero;
            }
        }
        public void Bind(StickerInstance inst)
        {
            instance = inst;
        }

        public ISticker GetLogic() => instance.Logic;
        public StickerData GetData() => instance.Data;

        public void SetRenderOnTop(bool enabled)
        {
            var mat = meshRenderer.material;

            if (enabled)
            {
                mat.renderQueue = 3000; // Transparent queue
                mat.SetInt("_ZWrite", 0);
                mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
            }
            else
            {
                mat.renderQueue = 2000; // Default
                mat.SetInt("_ZWrite", 1);
                mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
            }
        }

        public void DisableDragging() => CanDrag = false;

        public void Highlight(bool on)
        {
            if (!Interactable) return;
            if (highlighted == on)  return;

            
            highlighted = on;

            if (highlighted && coroutineReflection == null)
                coroutineReflection = StartCoroutine(PlayReflection(reflectionDuration));
        }

        public void SetBurningValue(float value)
        {
            meshRenderer.material.SetFloat("_BurnValue", value);
            meshRenderer.material.SetColor("_Multiply", Color.Lerp(Color.white, burnColor, value));

            HideStickerExtras();
        }

        public Coroutine Trigger(float speed = 1f) =>
            StartCoroutine(TriggerAnimation(speed));

        IEnumerator TriggerAnimation(float speed)
        {
            animator[ANIMATION_TRIGGER].speed = speed;
            animator.clip = animator.GetClip(ANIMATION_TRIGGER);
            animator.Play();
            SfxManager.Play(SfxClipId.StickerEvaluation);

            yield return null;

            float normalizedTime = animator[ANIMATION_TRIGGER].normalizedTime;

            while (normalizedTime < 0.8f && animator.isPlaying)
            {
                normalizedTime = animator[ANIMATION_TRIGGER].normalizedTime;
                yield return null;
            }
        }

        private IEnumerator PlayReflection(float duration)
        {
            float t = 0f;
            while (t < 1f)
            {
                t = Mathf.Clamp01(t + Time.deltaTime / duration);
                meshRenderer.material.SetFloat("_ReflectionValue", 1 - curveReflection.Evaluate(t));
                yield return null;
            }

            coroutineReflection = null;
        }

        void HideStickerExtras()
        {
            var meshes = meshRenderer
                         .GetComponentsInChildren<MeshRenderer>()
                         .Where(m => m != meshRenderer);

            foreach (var mesh in meshes) mesh.gameObject.SetActive(false);
        }

        void OnDestroy() => CombatEventManager.StickerDestroyed(this);
    }
}