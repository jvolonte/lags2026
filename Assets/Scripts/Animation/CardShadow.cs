using UnityEngine;

public class CardShadow : MonoBehaviour
{
    private const float MAX_RAYCASTDISTANCE = 50f;
    private const float HEIGHT_OFFSET = -0.01f;

    [SerializeField] Transform shadowSource;
    [SerializeField] Transform shadow;
    [SerializeField] LayerMask hitMask;

    RaycastHit hit;
    Vector3 scale;
    private void LateUpdate()
    {
        bool foundSurface = Physics.Raycast(
            shadowSource.position, 
            Vector3.down, 
            out hit, 
            MAX_RAYCASTDISTANCE, 
            hitMask);

        if (foundSurface != shadow.gameObject.activeSelf)
            shadow.gameObject.SetActive(foundSurface);

        if (foundSurface)
        {
            shadow.position = hit.point + Vector3.up * HEIGHT_OFFSET;
            shadow.rotation = Quaternion.LookRotation(hit.normal, shadowSource.up);
            scale.x = Mathf.Sqrt(1f - shadowSource.right.y * shadowSource.right.y);
            scale.y = Mathf.Sqrt(1f - shadowSource.up.y * shadowSource.up.y);
            scale.z = 1;

            shadow.localScale = scale;
        }
    }
}
