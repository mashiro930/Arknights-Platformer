using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C12_MoveState : IState
{
    private C12_FSM manager;
    private C12_Parameter c12_parameter;

    private int patrolPosition;

    public C12_MoveState(C12_FSM manager)
    {
        this.manager = manager;
        this.c12_parameter = manager.c12_parameter;
    }

    public void OnEnter()
    {
        c12_parameter.animator.Play("C12_Move");
    }

    public void OnUpdate()
    {
        manager.FlipTo(c12_parameter.patrolPoints[patrolPosition]);

        manager.transform.position = Vector2.MoveTowards(manager.transform.position, c12_parameter.patrolPoints[patrolPosition].position, c12_parameter.moveSpeed * Time.deltaTime);

        if (Vector2.Distance(manager.transform.position, c12_parameter.patrolPoints[patrolPosition].position) < 0.1f)
            manager.TransitionState(C12_StateType.Idle);

        if (c12_parameter.target != null &&
            manager.transform.position.x >= c12_parameter.chasePoints[0].position.x &&
            manager.transform.position.x <= c12_parameter.chasePoints[1].position.x)
            manager.TransitionState(C12_StateType.Chase);
    }

    public void OnExit()
    {
        patrolPosition++;

        if (patrolPosition >= c12_parameter.patrolPoints.Length)
            patrolPosition = 0;
    }
}
