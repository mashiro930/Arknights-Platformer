using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C12_ChaseState: IState
{
    private C12_FSM manager;
    private C12_Parameter c12_parameter;

    public C12_ChaseState(C12_FSM manager)
    {
        this.manager = manager;
        this.c12_parameter = manager.c12_parameter;
    }
    public void OnEnter()
    {
        c12_parameter.animator.Play("C12_Chase");
    }

    public void OnUpdate()
    {
        manager.FlipTo(c12_parameter.target);

        if (c12_parameter.target)
            manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                new Vector2(c12_parameter.target.position.x, manager.transform.position.y), c12_parameter.chaseSpeed * Time.deltaTime);

        if (c12_parameter.target == null || 
            manager.transform.position.x < c12_parameter.chasePoints[0].position.x ||
            manager.transform.position.x > c12_parameter.chasePoints[1].position.x)
            manager.TransitionState(C12_StateType.Idle);

        if (Physics2D.OverlapCircle(c12_parameter.attackPoint.position, c12_parameter.attackArea, c12_parameter.targetLayer))
            manager.TransitionState(C12_StateType.Attack);

    }

    public void OnExit()
    {

    }
}
