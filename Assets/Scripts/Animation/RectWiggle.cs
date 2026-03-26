using UnityEngine;

public class RectWiggle : MonoBehaviour
{
    public RectTransform rectTransform;
    public float angleRange;
    public float frameTime;

    float currentTime;
    float lastAngle;
    private void LateUpdate()
    {
        currentTime += Time.deltaTime;
        if (currentTime > frameTime)
        {
            float angle = Random.Range(angleRange * 0.5f, angleRange);
            if (Mathf.Sign(angle) == Mathf.Sign(lastAngle))
                angle = -angle;

            rectTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            currentTime -= frameTime;
            lastAngle = angle;
        }       
    }
}
