using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform toFollow;
    public bool interpolation = true;
    public bool useLateUpdate = false;
    public float yOffset = 1f;
    // Start is called before the first frame update
    //private float lastY = 0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!useLateUpdate)
        {
            Follow();
        }
        
    }

    void Follow()
    {
        if (interpolation)
        {
            transform.position = Vector3.Lerp(transform.position, toFollow.position + Vector3.up * yOffset, 5f * Time.deltaTime);
        } else
        {
            transform.position = toFollow.position + Vector3.up * yOffset;
        }
    }

    private void LateUpdate()
    {
        if (useLateUpdate)
        {
            Follow();
        }
    }
    /*void FixedUpdate()
    {
        float fixedAmount = Mathf.Abs(toFollow.position.y - lastY);
        if (fixedAmount < 0.1f)
        {                        
            Vector3 targetPosition = new Vector3(toFollow.position.x, lastY, toFollow.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime);
        } else
        {            
            transform.position = toFollow.position;
        }

        lastY = toFollow.position.y;        
    }*/
}
