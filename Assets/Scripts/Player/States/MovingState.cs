using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovingState : PlayerState
{
    [SerializeField] protected float speed = 4;
    [SerializeField] protected float jump = 5;

    public MovingState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Execution()
    {
        base.Execution();
    
        //movement
        Movement(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Jump(Input.GetButtonDown("Jump"));

        //switch to fight state
        SwitchFight(Input.GetButtonDown("Fire3"));
    }

    #region movement

    void Movement(float horizontal, float vertical)
    {
        //get direction by input
        Vector3 direction = Direction.WorldToLocalDirection(new Vector3(horizontal, 0, vertical), transform.rotation);

        //do movement
        DoMovement(direction, speed);
    }

    void Jump(bool inputJump)
    {
        //if press to jump and is grounded, jump (y axis)
        if (inputJump && player.IsGrounded)
        {
            rb.AddForce(transform.up * jump, ForceMode.VelocityChange);

            player.OnJump?.Invoke();
        }
    }

    #endregion

    protected virtual void SwitchFight(bool inputSwitch)
    {
        //if press input, switch to fight state
        if(inputSwitch)
        {
            player.SetState(player.fightState);

            //animation switch fight
            player.OnSwitchFight?.Invoke(true);
        }
    }
}
