using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class Player : MonoBehaviour
{
    [SerializeField] CameraBaseControl cameraControl;
    [Header("Movement")]
    [SerializeField] float speed = 4;
    [SerializeField] float jump = 10;
    [Header("Check Ground")]
    [SerializeField] Vector3 center;
    [SerializeField] Vector3 size;

    Transform cam;
    Rigidbody rb;

    //check in a box, if hit something other than the player
    bool isGrounded => Physics.OverlapBox(transform.position + center, size / 2, transform.rotation, CreateLayer.LayerAllExcept("Player"), QueryTriggerInteraction.Ignore).Length > 0;

    void Awake()
    {
        //get references
        cam = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        
        //set default camera
        cameraControl.StartDefault(cam, transform);
    }

    void Update()
    {
        //attacco
        //inserire lock sul nemico? usare SetRotation del cameraControl

        //movement
        Movement(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        MoveCamera();
        Rotate(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Jump(Input.GetButtonDown("Jump"));

        SwitchFight(Input.GetButtonDown("Fire3"));
    }

    private void OnDrawGizmosSelected()
    {
        //draw check ground
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + center, size);
    }

    #region private API

    #region movement

    void Movement(float horizontal, float vertical)
    {
        //get direction and current velocity (less y speed)
        Vector3 direction = Direction.WorldToLocalDirection(new Vector3(horizontal, 0, vertical), transform.rotation);
        Vector3 currentVelocity = rb.velocity - new Vector3(0, rb.velocity.y, 0);

        //new velocity with clamp
        Vector3 newVelocity = direction * speed - currentVelocity;
        newVelocity = Vector3.ClampMagnitude(newVelocity, speed);

        //set velocity (only x and y axis)
        rb.AddForce(newVelocity, ForceMode.VelocityChange);
    }

    void MoveCamera()
    {
        cameraControl.UpdateCameraPosition();
    }

    void Rotate(float inputX, float inputY)
    {
        cameraControl.UpdateRotation(inputX, inputY);
    }

    void Jump(bool inputJump)
    {
        //if press to jump and is grounded, jump (y axis)
        if(inputJump && isGrounded)
        {
            rb.AddForce(transform.up * jump, ForceMode.VelocityChange);
        }
    }

    #endregion

    void SwitchFight(bool inputSwitch)
    {
        if(inputSwitch)
        {
            //passa da combattimenti a pugni a combattimenti di spada
        }
    }

    #endregion
}
