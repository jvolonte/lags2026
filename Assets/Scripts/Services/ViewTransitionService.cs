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

        void CopyVisual(Transform source, Transform proxy)
        {
            var srcRenderer = source.GetComponentInChildren<MeshRenderer>();
            var dstRenderer = proxy.GetComponentInChildren<MeshRenderer>();

            if (srcRenderer && dstRenderer)
                dstRenderer.material = srcRenderer.material;
        }
    }
}