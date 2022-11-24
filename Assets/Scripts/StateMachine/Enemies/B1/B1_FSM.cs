using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum B1_StateType
{
    Idle, Move, Die
}

[Serializable]
public class B1_Parameter
{
    public int health;

    public float moveSpeed;
    public float chaseSpeed;
    public float idleTime;

    public Transform[] patrolPoints;
    public Transform[] chasePoints;

    public Animator animator;

    public bool getHit = false;
}

public class B1_FSM : MonoBehaviour
{
    public B1_Parameter parameter;

    private IState currentState;

    private Dictionary<B1_StateType, IState> states = new Dictionary<B1_StateType, IState>();

    // Start is called before the first frame update
    void Start()
    {
        states.Add(B1_StateType.Idle, new B1_IdleState(this));
        states.Add(B1_StateType.Move, new B1_MoveState(this));
        states.Add(B1_StateType.Die, new B1_DieState(this));

        TransitionState(B1_StateType.Idle);

        parameter.animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        currentState.OnUpdate();
    }

    public void TransitionState(B1_StateType type)
    {
        if (currentState != null)
            currentState.OnExit();
        currentState = states[type];
        currentState.OnEnter();
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
        if (other.CompareTag("PlayerAttack"))
            parameter.getHit = true;
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
