using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

public class ActorMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    float _moveSpeed = 5f;
    float _gravity = 0.3f;
    public float hMovement;
    public float vMovement;

    Vector3 _moveDirection;
    CharacterController _characterController;    
    AnimationStateController modelAnimator;
    ActorController _actorController;

    public NavMeshAgent _navMeshAgent;

    float angle = 0f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    void Awake()
    {
        _characterController = transform.GetComponent<CharacterController>();
        _navMeshAgent = transform.GetComponent<NavMeshAgent>();        
        _navMeshAgent.enabled = false;
        _actorController = transform.parent.gameObject.GetComponent<ActorController>();
        modelAnimator = transform.Find("CharacterModel").GetComponent<AnimationStateController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_actorController.controllable)
        {
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        } else
        {
            if (hMovement != 0 || vMovement != 0)
            {
                float targetAngle = Camera.main.transform.eulerAngles.y;
                if (hMovement != 0)
                {
                    if (hMovement > 0)
                    {
                        if (vMovement > 0)
                        {
                            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle + 30f, ref turnSmoothVelocity, turnSmoothTime);
                        }
                        else if (vMovement < 0)
                        {
                            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle - 30f, ref turnSmoothVelocity, turnSmoothTime);
                        }
                        else
                        {
                            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle + 90f, ref turnSmoothVelocity, turnSmoothTime);
                        }

                    }
                    else
                    {
                        if (vMovement > 0)
                        {
                            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle - 30f, ref turnSmoothVelocity, turnSmoothTime);
                        }
                        else if (vMovement < 0)
                        {
                            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle + 30f, ref turnSmoothVelocity, turnSmoothTime);
                        }
                        else
                        {
                            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle - 90f, ref turnSmoothVelocity, turnSmoothTime);
                        }

                    }

                }
                else
                {
                    angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                }
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

            }
        }        
    }

    void FixedUpdate()
    {
        _handleMovement();
    }

    void SendMovement(Vector3 transformDirection, float moveSpeed, float vMovement)
    {
        if (_actorController.controllable && _actorController.playerId == Client.instance.clientId)
        {
            ClientSend.Movement(transformDirection, moveSpeed, vMovement, Time.deltaTime, Mathf.Floor(angle));
        }        
    }   
    void _handleMovement()
    {
        vMovement = ClampMove(vMovement);
        hMovement = ClampMove(hMovement);
        /*if (hMovement != 0 && vMovement != 0)
        {
            hMovement = 0.65f * hMovement;
            vMovement = 0.65f * vMovement;
            
        }*/
        modelAnimator.velocityZ = vMovement;

        Vector3 inputDirection = new Vector3(0f, 0f, vMovement);
        if (hMovement != 0)
        {
            if (vMovement > 0)
            {
                inputDirection = new Vector3(0f, 0f, vMovement);
            }
            else if (vMovement < 0)
            {
                inputDirection = new Vector3(0f, 0f, vMovement);
            }
            else
            {
                inputDirection = new Vector3(0f, 0f, Mathf.Abs(hMovement));
                modelAnimator.velocityZ = Mathf.Abs(hMovement);
            }
        }

        Vector3 transformDirection = transform.TransformDirection(inputDirection);        
        Vector3 flatMovement = AdjustSpeedMultiplier(_moveSpeed) * Time.deltaTime * transformDirection;
        /*if(modelAnimator.velocityZ != 0)
        {
            
        }*/
        SendMovement(transformDirection, AdjustSpeedMultiplier(_moveSpeed), modelAnimator.velocityZ);
        //_moveDirection = new Vector3(flatMovement.x, _moveDirection.y, flatMovement.z);

        if (!_characterController.isGrounded)
        {
            _moveDirection.y -= _gravity * Time.deltaTime;
        }
        else
        {
            _moveDirection.y = 0f;
        }

        _characterController.Move(_moveDirection);
        //_moveDirection = new Vector3(0, 0, 0);

        if (!_navMeshAgent.pathPending && _navMeshAgent.enabled)
        {
            modelAnimator.velocityZ = 1;
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    // Done
                    _navMeshAgent.enabled = false;
                    modelAnimator.velocityZ = 0;
                }
            }
            if (hMovement != 0 || vMovement != 0)
            {
                _navMeshAgent.enabled = false;
            }
        }        
    }
    public void Move(Vector3 position, float vMvment, float agl)
    {
        _moveDirection = new Vector3(position.x, _moveDirection.y, position.z);
        angle = agl;
        vMovement = vMvment;
    }
    float ClampMove(float value)
    {
        if (value > 0f)
        {
            return 1f;
        }
        if (value < 0f)
        {
            return -1f;
        }
        return 0f;
    }

    float AdjustSpeedMultiplier(float speed)
    {
        float adjustedMultiplier = speed;
        /*if(hMovement != 0 && vMovement != 0)
        {
            adjustedMultiplier = speed / 1.25f;
        }*/
        if (vMovement < -0)
        {
            adjustedMultiplier = speed / 1.25f;
        }
        return Mathf.Clamp(adjustedMultiplier, 0f, 10f);
    }
}
