using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_DieState : IState
{
    private B1_FSM manager;
    private B1_Parameter parameter;

    private AnimatorStateInfo b1_info;

    public B1_DieState(B1_FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("B1_Die");
    }

    public void OnUpdate()
    {
        b1_info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (b1_info.normalizedTime >= .95f)
            manager.SelfDestroy();

    }

    public void OnExit()
    {

    }

}