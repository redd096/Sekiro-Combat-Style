using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitState : State
{
    float timeToWait;
    State nextState;
    System.Action func;

    public WaitState(StateMachine stateMachine, float timeToWait, State nextState, System.Action func = null) : base(stateMachine)
    {
        //set time to wait and next state
        this.timeToWait = timeToWait;
        this.nextState = nextState;
        this.func = func;
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
