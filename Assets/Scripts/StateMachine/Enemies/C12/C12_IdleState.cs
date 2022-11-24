using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C12_IdleState : IState
{
    private C12_FSM manager;
    private C12_Parameter c12_parameter;

    private float timer;

    public C12_IdleState(C12_FSM manager)
    {
        this.manager = manager;
        this.c12_parameter = manager.c12_parameter;
    }

    public void OnEnter()
    {
        c12_parameter.animator.Play("C12_Idle");
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;

        if (c12_parameter.target != null &&
            manager.transform.position.x >= c12_parameter.chasePoints[0].position.x &&
            manager.transform.position.x <= c12_parameter.chasePoints[1].position.x)
            manager.TransitionState(C12_StateType.Chase);

        if (timer >= c12_parameter.idleTime)
            manager.TransitionState(C12_StateType.Move);
    }

    public void OnExit()
    {
        timer = 0;
    }

}
