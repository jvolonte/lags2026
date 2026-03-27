using Data.Stickers;
using Stickers;
using UnityEngine;

namespace Views
{
    public class StickerView : MonoBehaviour
    {
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] Texture burnNoise;
        [SerializeField] Color burnEdgeColor;
        [SerializeField] Color burnColor;

        StickerInstance instance;
        public MeshRenderer MeshRenderer => meshRenderer;

        public bool Dragging { get; set; } = false;
        public bool CanDrag { get; private set; } = true;

        private void Awake()
        {
            meshRenderer.material.SetTexture("_BurnNoise", burnNoise);
            meshRenderer.material.SetColor("_Burn", burnEdgeColor);
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

        public void SetBurningValue(float value)
        {
            meshRenderer.material.SetFloat("_BurnValue", value);
            meshRenderer.material.SetColor("_Multiply", Color.Lerp(Color.white, burnColor, value));
        }

        void OnDestroy() => CombatEventManager.StickerDestroyed(this);
    }
}