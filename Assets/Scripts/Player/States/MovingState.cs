using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovingState : PlayerState
{
    [SerializeField] protected float speed = 4;
    [SerializeField] protected float jump = 5;

    Rigidbody rb;

    public MovingState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Awake(StateMachine stateMachine)
    {
        base.Awake(stateMachine);

        //get references
        rb = transform.GetComponent<Rigidbody>();
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
        //get direction and current velocity (less y speed)
        Vector3 direction = Direction.WorldToLocalDirection(new Vector3(horizontal, 0, vertical), transform.rotation);
        Vector3 currentVelocity = rb.velocity - new Vector3(0, rb.velocity.y, 0);

        //new velocity with clamp
        Vector3 newVelocity = direction * speed - currentVelocity;
        newVelocity = Vector3.ClampMagnitude(newVelocity, speed);

        //set velocity (only x and z axis)
        rb.AddForce(newVelocity, ForceMode.VelocityChange);
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
