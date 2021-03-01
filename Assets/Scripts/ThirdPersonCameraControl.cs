using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class ThirdPersonCameraControl : MonoBehaviour
{
    public float TouchSensitivity_x = 10f;
    public float TouchSensitivity_y = 10f;

    public float cameraDistanceMax = 13f;
    public float cameraDistanceMin = 5f;
    public float cameraDistance = 7f;
    public float scrollSpeed = 0.5f;

    public CinemachineFreeLook freeLookCam;
    ActorController m_ActorController;

    private float snapAngle;
    private bool doSnapCamera;
    // Use this for initialization
    void Start()
    {
        m_ActorController = gameObject.GetComponent<ActorController>();
        CinemachineCore.GetInputAxis = this.HandleAxisInputDelegate;
        doSnapCamera = false;

        if(m_ActorController.controllable)
        {
            //freeLookCam = GameObject.Find("PlayerCamera").GetComponent<CinemachineFreeLook>();
            freeLookCam.m_Orbits[1].m_Radius = cameraDistance;
        }
        m_ActorController.OnControllableChange += PlayerControllableChangeHandler;
    }

    private void PlayerControllableChangeHandler(bool newState)
    {
        if (newState)
        {
            //freeLookCam = GameObject.Find("PlayerCamera").GetComponent<CinemachineFreeLook>();
            freeLookCam.m_Orbits[1].m_Radius = cameraDistance;
        }
    }

    private float HandleAxisInputDelegate(string axisName)
    {
        switch (axisName)
        {
            case "Mouse X":
                if (Input.touchCount > 0)
                {
                    //Is mobile touch
                    return Input.touches[0].deltaPosition.x / TouchSensitivity_x;
                }
                else if (Input.GetMouseButton(1))
                {
                    // is mouse click
                    return Input.GetAxis("Mouse X");
                }
                break;
            case "Mouse Y":
                if (Input.touchCount > 0)
                {
                    //Is mobile touch
                    return Input.touches[0].deltaPosition.y / TouchSensitivity_y;
                }
                else if (Input.GetMouseButton(1))
                {
                    // is mouse click
                    return Input.GetAxis(axisName);
                }
                break;
            default:
                Debug.LogError("Input <" + axisName + "> not recognized.", this);
                break;
        }

        return 0f;
    }

    void Update()
    {
        if (m_ActorController.controllable) { 
            if (Input.GetMouseButtonDown(2))
            {
                snapAngle = freeLookCam.m_XAxis.Value + 180f;
                doSnapCamera = true;
            }
            cameraDistance += Input.mouseScrollDelta.y * scrollSpeed;
            cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);
        }
    }
    void HandleSwingCamera()
    {
        if (Input.GetKey("q"))
        {
            freeLookCam.m_XAxis.Value += -2f;
        }
        if (Input.GetKey("e"))
        {
            freeLookCam.m_XAxis.Value += 2f;
        }
    }

    void LateUpdate()
    {

        if (m_ActorController.controllable) { 
            if (doSnapCamera)
            {
            
                freeLookCam.m_XAxis.Value = Quaternion.Lerp(Quaternion.Euler(0, freeLookCam.m_XAxis.Value, 0), Quaternion.Euler(0, snapAngle, 0), 5 * Time.deltaTime).eulerAngles.y;

                if (Mathf.Abs(freeLookCam.m_XAxis.Value - snapAngle) < 1f)
                {

                    doSnapCamera = false;

                }
            }
            freeLookCam.m_Orbits[1].m_Radius = cameraDistance;
            HandleSwingCamera();
        }
    }
}
