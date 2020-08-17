using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class PlayerGraphics : MonoBehaviour
{
    [SerializeField] float smooth = 5;

    Player player;
    Animator anim;
    Rigidbody rb;

    Vector3 previousSpeed;
    bool canJump;

    void Start()
    {
        //get references
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //movement
        Movement();
        Jump();
        Falling();

        SwitchFight(Input.GetButtonDown("Fire3"));
    }

    #region private API

    #region movement

    void Movement()
    {
        //smooth between previous speed to new speed, for movement animation
        Vector3 localVelocity = Direction.WorldToInverseLocalDirection(rb.velocity, transform.rotation).normalized;
        Vector3 newSpeed = Vector3.Lerp(previousSpeed, localVelocity, Time.deltaTime * smooth);

        //save previous speed
        previousSpeed = newSpeed;

        //set animator
        anim.SetFloat("Horizontal", newSpeed.x);
        anim.SetFloat("Vertical", newSpeed.z);
    }

    void Jump()
    {
        //if rigidbody is going up and player can jump, do it
        if (rb.velocity.y > 0.5f && canJump)
        {
            anim.SetTrigger("Jump");
            canJump = false;
        }

        //check if falling or grounded, so can jump again
        if(rb.velocity.y <= 0 && canJump == false)
        {
            canJump = true;
        }
    }

    void Falling()
    {
        //set if falling
        if (rb.velocity.y < -0.5f)
            anim.SetBool("Falling", true);
        else
            anim.SetBool("Falling", false);
    }

    #endregion

    void SwitchFight(bool inputSwitch)
    {
        //switch from unarmed to sword
        if (inputSwitch)
        {
            anim.SetBool("Unarmed", !anim.GetBool("Unarmed"));
            anim.SetTrigger("SwitchFight");
        }
    }

    #endregion
}
