using Stickers;
using UnityEngine;

namespace Views
{
    public class StickerView : MonoBehaviour
    {
        [SerializeField] MeshRenderer meshRenderer;

        StickerInstance instance;

        public void Bind(StickerInstance inst)
        {
            instance = inst;
            // meshRenderer.material.SetTexture("_MainTex", inst.Data.texture);
        }

        public ISticker GetLogic() => instance.Logic;
    }
}