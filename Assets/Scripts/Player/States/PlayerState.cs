using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class PlayerState : State
{
    protected Player player;
    protected Transform transform;
    protected Rigidbody rb;
    protected CameraBaseControl cameraControl;

    public PlayerState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Awake(StateMachine stateMachine)
    {
        base.Awake(stateMachine);

        //get references
        player = stateMachine as Player;
        transform = player.transform;
        rb = transform.GetComponent<Rigidbody>();
        cameraControl = player.cameraControl;
    }

    public override void Execution()
    {
        base.Execution();

        //camera update
        MoveCamera();
        Rotate(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    #region private API

    void MoveCamera()
    {
        //move camera
        cameraControl.UpdateCameraPosition();
    }

    void Rotate(float inputX, float inputY)
    {
        //rotate player and camera
        cameraControl.UpdateRotation(inputX, inputY);
    }

    protected void LookEnemy()
    {
        //look enemy
        Transform target = player.GetTarget("Enemy");

        if (target)
        {
            Vector3 lookEnemy = target.position - transform.position;
            cameraControl.SetRotation(Quaternion.LookRotation(lookEnemy));
        }
    }

    protected void DoMovement(Vector3 direction, float speed)
    {
        //get current velocity (less y speed)
        Vector3 currentVelocity = rb.velocity - new Vector3(0, rb.velocity.y, 0);

        //new velocity with clamp
        Vector3 newVelocity = direction * speed - currentVelocity;
        newVelocity = Vector3.ClampMagnitude(newVelocity, speed);

        //set velocity (only x and z axis)
        rb.AddForce(newVelocity, ForceMode.VelocityChange);
    }

    protected void StopMovement()
    {
        //get current velocity (less y speed)
        Vector3 currentVelocity = rb.velocity - new Vector3(0, rb.velocity.y, 0);

        //set velocity (only x and z axis)
        rb.AddForce(-currentVelocity, ForceMode.VelocityChange);
    }

    #endregion
}
