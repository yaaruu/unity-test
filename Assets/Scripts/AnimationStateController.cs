using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;        

    public float velocityX = 0f;
    public float velocityZ = 0f;
    public bool Jumping = false;
    int VelXHash;
    int VelZHash;
    int JumpHash;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        VelXHash = Animator.StringToHash("Velocity X");
        VelZHash = Animator.StringToHash("Velocity Z");
        JumpHash = Animator.StringToHash("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat(VelXHash, velocityX);
        animator.SetFloat(VelZHash, velocityZ);
        animator.SetBool(JumpHash, Jumping);
    }
}
