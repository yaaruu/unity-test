using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonMovement : MonoBehaviour
{    
    public Rigidbody playerBody;    
    public Collider playerCollider;
        
    [SerializeField]
    public float speedMultiplier = 10f;

    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    public float maxVelocityChange = 10.0f;    

    private float angle;
    public float playerGravity = 0.98f;

    private float distToGround;

    [HideInInspector]
    public float hMovement;
    [HideInInspector]
    public float vMovement;
    
    float clickRotAngle;    
    bool shouldClickRotate = false;    
    Vector3 clickPoint;
    [HideInInspector]
    public bool shouldClickMove = false;

    [HideInInspector]
    public bool _jump = false;
    [HideInInspector]
    public bool _canJump = true;

    [HideInInspector]
    public float targetAngle;

    [HideInInspector]
    public AnimationStateController modelAnimator;
    

    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        modelAnimator = GameObject.Find("CharacterModel").GetComponent<AnimationStateController>();                

    }
    // Update is called once per frame
    void Update()
    {
        angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);                
        if (hMovement != 0 || vMovement != 0)
        {
            shouldClickRotate = false;
            shouldClickMove = false;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        
        if (Mathf.Abs(transform.eulerAngles.y) != Mathf.Abs(clickRotAngle) && shouldClickRotate == true)
        {                    
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, clickRotAngle, 0f), 500f * Time.deltaTime);
        }
        if (shouldClickRotate)
        {
            Vector3 relativePos = clickPoint - transform.position;
            relativePos.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(relativePos, Vector3.up);
            clickRotAngle = targetRotation.eulerAngles.y;
            float rotDiff = Mathf.Abs(transform.eulerAngles.y) - Mathf.Abs(clickRotAngle);
            if (Mathf.Abs(rotDiff) < 0.01f)
            {
                shouldClickRotate = false;
            }
        }

        if (shouldClickMove)
        {
            float dist = Vector3.Distance(clickPoint, transform.position);            
            if (Mathf.Abs(dist) <= 1f)
            {
                shouldClickMove = false;
            }
        }

        
    }

    float getFacing() => transform.eulerAngles.y;    

    bool isGrounded() => Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.2f);    

    public void Move(Vector3 targetPos)
    {
        Vector3 relativePos = targetPos - transform.position;
        relativePos.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(relativePos, Vector3.up);
        clickRotAngle = targetRotation.eulerAngles.y;
        clickPoint = targetPos;
        shouldClickRotate = true;
        shouldClickMove = true;
    }

    void FixedUpdate()
    {                
        Vector3 targetVelocity = new Vector3(hMovement, 0.5f, vMovement);
        targetVelocity = transform.TransformDirection(targetVelocity);

        

        float adjustedSpeedMultiplier = speedMultiplier;
        if (!isGrounded())
        {
            adjustedSpeedMultiplier = speedMultiplier;
            playerBody.AddForce(Vector3.down * (playerGravity / 2f), ForceMode.Impulse);
        }
        if (hMovement != 0 && vMovement != 0)
        {
            adjustedSpeedMultiplier = speedMultiplier / 1.5f;
        }
        // handle backward movement
        if (vMovement < -0.5)
        {
            adjustedSpeedMultiplier = speedMultiplier / 1.75f;
        }
        targetVelocity *= adjustedSpeedMultiplier;
        
        

        Vector3 velocity = playerBody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);        
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        

        modelAnimator.velocityZ = hMovement;
        modelAnimator.velocityX = vMovement;

        if (shouldClickMove)
        {
            modelAnimator.velocityX = 1f;
            playerBody.AddRelativeForce(Vector3.forward * Mathf.Clamp(5f, -maxVelocityChange, maxVelocityChange), ForceMode.Impulse);
        }

        if(_jump)
        {
            
            _jump = false;
            StartCoroutine(startJump());
            StartCoroutine(enableJump());
        }

        playerBody.AddForce(velocityChange, ForceMode.Impulse);

        

        // update player distance to the ground        
        distToGround = playerCollider.bounds.extents.y;

        
    }

    IEnumerator startJump()
    {
        yield return new WaitForSeconds(0.1f);
        playerBody.AddRelativeForce(Vector3.up * 10f, ForceMode.Impulse);
    }

    IEnumerator enableJump()
    {
        yield return new WaitForSeconds(1f);
        _canJump = true;
        modelAnimator.Jumping = false;
    }
}
