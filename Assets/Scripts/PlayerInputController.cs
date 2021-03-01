using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{    
    [SerializeField]
    ActorMovement playerMovement;
    [SerializeField]
    ActorController m_ActorController;

    private Camera m_MainCamera;

    float hMovement;
    float vMovement;
    void Start()
    {
        playerMovement = gameObject.GetComponentInChildren<ActorMovement>();
        m_ActorController = gameObject.GetComponent<ActorController>();
        m_MainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (m_ActorController.controllable)
        {            
            hMovement = Input.GetAxis("Horizontal");
            vMovement = Input.GetAxis("Vertical");
            playerMovement.hMovement = hMovement;
            playerMovement.vMovement = vMovement;
            
            // Click To Move
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    Vector3 targetPos = hit.point;
                    playerMovement._navMeshAgent.enabled = true;
                    playerMovement._navMeshAgent.SetDestination(targetPos);
                }
            }

            /*if (Input.GetKey("v") && playerMovement._canJump && !playerMovement.shouldClickMove)
            {
                playerMovement._jump = true;
                playerMovement._canJump = false;
                playerMovement.modelAnimator.Jumping = true;
            }*/
        }        
    }
}
