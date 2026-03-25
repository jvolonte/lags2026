using UnityEngine;

namespace Views
{
    public class BillboardView : MonoBehaviour
    {
        Camera mainCamera;

        void Start()
        {
            mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            if (mainCamera != null)
            {
                transform.LookAt(mainCamera.transform);
                transform.localEulerAngles += new Vector3(0, 180, 0);
            }
        }
    }
}