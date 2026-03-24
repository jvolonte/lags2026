using UnityEngine;

namespace Views
{
    public class StickerSurface : MonoBehaviour
    {
        [SerializeField] MeshRenderer meshRenderer;

        public Bounds Bounds => meshRenderer.bounds;

        public Vector3 GetCenter() => Bounds.center;

        public Vector3 GetUp() => transform.up;
        public Vector3 GetRight() => transform.right;
    }
}