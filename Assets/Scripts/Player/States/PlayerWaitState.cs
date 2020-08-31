using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWaitState : PlayerState
{
    float timeToWait;
    State nextState;
    System.Action func;

    Coroutine wait_Coroutine;

    public PlayerWaitState(StateMachine stateMachine, float timeToWait, State nextState, System.Action func = null) : base(stateMachine)
    {
        //set time to wait and next state
        this.timeToWait = timeToWait;
        this.nextState = nextState;
        this.func = func;
    }

    public override void Enter()
    {
        base.Enter();

        //be sure to stop movement
        StopMovement();

        //start wait coroutine
        wait_Coroutine = player.StartCoroutine(Wait_Coroutine());
    }

    public override void Execution()
    {
        base.Execution();

        //lock cam to enemy
        LookEnemy();
    }

    public override void Exit()
    {
        base.Exit();

        //be sure to stop coroutines
        if (wait_Coroutine != null)
            player.StopCoroutine(wait_Coroutine);
    }

    IEnumerator Wait_Coroutine()
    {
        //wait
        yield return new WaitForSeconds(timeToWait);

        //invoke function
        func?.Invoke();

        //go to next state
        stateMachine.SetState(nextState);
    }
}
