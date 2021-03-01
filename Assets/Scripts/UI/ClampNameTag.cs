using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampNameTag : MonoBehaviour
{
    public Transform toFollow;

    private void LateUpdate()
    {
        Vector3 targetPos = Camera.main.WorldToScreenPoint(toFollow.position);
        transform.position = targetPos;
    }
}
