using UnityEngine;

namespace Animation
{
    public class NervousShake : MonoBehaviour
    {
        public float positionAmplitude = 0.02f;
        public float rotationAmplitude = 1.5f;
        public float frequency = 6f;

        Vector3 basePos;
        Quaternion baseRot;

        void Start()
        {
            basePos = transform.localPosition;
            baseRot = transform.localRotation;
        }

        void Update()
        {
            float t = Time.time * frequency;

            float noiseX = Mathf.PerlinNoise(t, 0f) - 0.5f;
            float noiseY = Mathf.PerlinNoise(0f, t) - 0.5f;
            float noiseZ = Mathf.PerlinNoise(t, t) - 0.5f;

            Vector3 posOffset = new Vector3(noiseX, noiseY, noiseZ) * positionAmplitude;
            Vector3 rotOffset = new Vector3(noiseY, noiseZ, noiseX) * rotationAmplitude;

            transform.localPosition = basePos + posOffset;
            transform.localRotation = baseRot * Quaternion.Euler(rotOffset);
        }
    }
}