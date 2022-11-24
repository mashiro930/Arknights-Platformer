using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_IdleState : IState
{
    private B1_FSM manager;
    private B1_Parameter parameter;

    private float timer;
    //private int animID = Animator.StringToHash("B1Idle");

    public B1_IdleState(B1_FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("B1_Idle");
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

        timer += Time.deltaTime;

        if (timer >= parameter.idleTime)
            manager.TransitionState(B1_StateType.Move);
    }

    public void OnExit()
    {
        timer = 0;
        parameter.getHit = false;
    }

}
