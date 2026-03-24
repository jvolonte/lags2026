using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera target;
    public bool ignoreHeight;
    private void Awake()
    {
        if (target == null)
            target = Camera.main;
    }

    private void LateUpdate()
    {
        Vector3 pos = target.transform.position;
        if (ignoreHeight) pos.y = this.transform.position.y;

        transform.LookAt(pos);
    }
}
