using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWaitState : PlayerState
{
    float timeToWait;
    State nextState;
    System.Action func;

    public PlayerWaitState(StateMachine stateMachine, float timeToWait, State nextState, System.Action func = null) : base(stateMachine)
    {
        //set time to wait and next state
        this.timeToWait = timeToWait;
        this.nextState = nextState;
        this.func = func;
    }

    public override void Awake(StateMachine stateMachine)
    {
        base.Awake(stateMachine);

        //be sure to stop movement
        StopMovement();
    }

    public override void Execution()
    {
        //stop also camera rotation present in PlayerState
    }

    public override IEnumerator Enter()
    {
        yield return base.Enter();

        //wait
        yield return new WaitForSeconds(timeToWait);

        //go to next state
        stateMachine.SetState(nextState);

        //invoke function
        func?.Invoke();
    }
}
