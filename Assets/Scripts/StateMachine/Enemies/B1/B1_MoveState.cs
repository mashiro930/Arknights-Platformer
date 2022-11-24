using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_MoveState : IState
{
    private B1_FSM manager;
    private B1_Parameter parameter;

    private int patrolPosition;
    //private int animID = Animator.StringToHash("B1Move");
    public B1_MoveState(B1_FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("B1_Move");
    }

    public void OnUpdate()
    {
        if (parameter.getHit)
        {
            parameter.health--;

            if (parameter.health > 0)
                manager.TransitionState(B1_StateType.Idle);
            if (parameter.health == 0)
                manager.TransitionState(B1_StateType.Die);
        }

        manager.FlipTo(parameter.patrolPoints[patrolPosition]);

        manager.transform.position = Vector2.MoveTowards(manager.transform.position, parameter.patrolPoints[patrolPosition].position, parameter.moveSpeed * Time.deltaTime);

        if (Vector2.Distance(manager.transform.position, parameter.patrolPoints[patrolPosition].position) < 0.1f)
            manager.TransitionState(B1_StateType.Idle);
    }

    public void OnExit()
    {
        patrolPosition++;

        if (patrolPosition >= parameter.patrolPoints.Length)
            patrolPosition = 0;

        parameter.getHit = false;
    }
}
