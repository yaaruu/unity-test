using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    
    public ActorMovement playerMovement;
    public string playerName;
    public int playerId;
    [SerializeField]
    private bool m_Controllable = false;
    [SerializeField]
    public bool controllable
    {
        get { return m_Controllable; }
        set
        {
            if (m_Controllable == value) return;
            m_Controllable = value;
            if (OnControllableChange != null)
                OnControllableChange(m_Controllable);
        }
    }

    public delegate void OnControllableChangeDelegate(bool newVal);
    public event OnControllableChangeDelegate OnControllableChange;

    void Awake()
    {
        playerMovement = transform.GetComponentInChildren<ActorMovement>();
    }

    public void Move(Vector3 targetPos, float vMovement,float angle)
    {
        playerMovement.Move(targetPos, vMovement, angle);
    }
}
