using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C12_AttackState : IState
{
    private C12_FSM manager;
    private C12_Parameter c12_parameter;

    private AnimatorStateInfo c12_info;

    public C12_AttackState(C12_FSM manager)
    {
        this.manager = manager;
        this.c12_parameter = manager.c12_parameter;
    }

    public void OnEnter()
    {
        c12_parameter.animator.Play("C12_Attack");
    }

    public void OnUpdate()
    {
        c12_info = c12_parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (c12_info.normalizedTime >= .95f)
            manager.TransitionState(C12_StateType.Chase);

    }

    public void OnExit()
    {
        
    }

}
