using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum C12_StateType
{
    Idle, Move, Chase,Attack
}

[Serializable]
public class C12_Parameter
{
    public int health;

    public float moveSpeed;
    public float chaseSpeed;
    public float idleTime;

    public Transform[] patrolPoints;
    public Transform[] chasePoints;

    public Transform target;

    public LayerMask targetLayer;
    public Transform attackPoint;
    public float attackArea;

    public Animator animator;
}

public class C12_FSM : MonoBehaviour
{
    public C12_Parameter c12_parameter;

    private IState c12_currentState;

    private Dictionary<C12_StateType, IState> c12_states = new Dictionary<C12_StateType, IState>();

    // Start is called before the first frame update
    void Start()
    {
        c12_states.Add(C12_StateType.Idle, new C12_IdleState(this));
        c12_states.Add(C12_StateType.Move, new C12_MoveState(this));
        c12_states.Add(C12_StateType.Chase, new C12_ChaseState(this));
        c12_states.Add(C12_StateType.Attack, new C12_AttackState(this));

        TransitionState(C12_StateType.Idle);

        c12_parameter.animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        c12_currentState.OnUpdate();
    }

    public void TransitionState(C12_StateType type)
    {
        if (c12_currentState != null)
            c12_currentState.OnExit();
        c12_currentState = c12_states[type];
        c12_currentState.OnEnter();
    }

    public void FlipTo(Transform target)
    {
        if(target != null)
        {
            if (transform.position.x > target.position.x)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (transform.position.x < target.position.x)
                transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            c12_parameter.target = other.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            c12_parameter.target = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(c12_parameter.attackPoint.position, c12_parameter.attackArea);
    }
}
