using DG.Tweening;
using UnityEngine;

namespace Views
{
    public class HeartView : MonoBehaviour
    {
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] Texture[] fullHearts;
        [SerializeField] Texture emptyHeart;
        [SerializeField] ParticleSystem particlesOnHurt;

        Texture fullHeart;

        [Header("Animation")] 
        [SerializeField] float floatAmplitude = 0.05f;
        [SerializeField] float floatDuration = 1.2f;

        bool isFilled;
        Vector3 baseLocalPos;

        void Awake()
        {
            fullHeart = fullHearts.PickOne();
            // meshRenderer.material = Instantiate(meshRenderer.material);
            meshRenderer.material.SetTexture("_MainTex", fullHeart);
            baseLocalPos = transform.localPosition;
            isFilled = true;
        }

        void Start()
        {
            StartFloating();
            
            transform.DOLocalRotate(new Vector3(0, 0, Random.Range(-15f, 15f)), 0f);
        }

        void StartFloating()
        {
            var offset = Random.Range(0f, 1f);

            transform.DOLocalMoveY(baseLocalPos.y + floatAmplitude, floatDuration)
                     .SetEase(Ease.InOutSine)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetDelay(offset);
        }

        public void SetFilled(bool filled)
        {
            if (isFilled == filled)
                return;

            isFilled = filled;

            if (!isFilled )
                particlesOnHurt.Play();

            var texture = filled ? fullHeart : emptyHeart;
            meshRenderer.material.SetTexture("_MainTex", texture);

            if (!filled)
                PlayLoseAnimation();
        }

        void PlayLoseAnimation() => transform.DOPunchScale(Vector3.one * 0.3f, 0.25f, 10, 1);
    }
}