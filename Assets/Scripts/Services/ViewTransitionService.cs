using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Services
{
    public class ViewTransitionService : MonoBehaviour
    {
        [SerializeField] float defaultDuration = 0.25f;
        [SerializeField] Ease ease = Ease.InOutQuad;

        public IEnumerator MoveAndSwap(
            Transform source,
            Transform target,
            GameObject proxyPrefab,
            System.Action onArrive,
            float? durationOverride = null
        )
        {
            var duration = durationOverride ?? defaultDuration;
            var proxy = Instantiate(proxyPrefab, source.position, source.rotation);
            DisableInteractiveLayers(proxy);
            SetLayerRecursively(proxy, LayerMask.NameToLayer("Ignore Raycast"));
            CopyVisual(source, proxy.transform);
            source.gameObject.SetActive(false);

            yield return DOTween.Sequence()
                                .Join(proxy.transform.DOMove(target.position, duration).SetEase(ease))
                                .Join(proxy.transform.DORotateQuaternion(target.rotation, duration))
                                .Join(proxy.transform.DOScale(target.localScale, duration))
                                .WaitForCompletion();

            onArrive?.Invoke();
            Destroy(proxy);
        }

        void DisableInteractiveLayers(GameObject obj)
        {
            var colliders = obj.GetComponentsInChildren<Collider>(true);
            foreach (var col in colliders) col.enabled = false;

            var colliders2D = obj.GetComponentsInChildren<Collider2D>(true);
            foreach (var col in colliders2D) col.enabled = false;
        }

        void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
                SetLayerRecursively(child.gameObject, layer);
        }

        void CopyVisual(Transform source, Transform proxy)
        {
            var srcRenderer = source.GetComponentInChildren<MeshRenderer>();
            var dstRenderer = proxy.GetComponentInChildren<MeshRenderer>();

            if (srcRenderer && dstRenderer)
                dstRenderer.material = srcRenderer.material;
        }
    }
}