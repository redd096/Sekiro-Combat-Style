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
    bool isGrounded;

    void Start()
    {
        //get references
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Debug.Log(rb.velocity);

        Movement();
        Jump();
        Falling();

        SwitchFight(Input.GetButtonDown("Fire3"));
    }

    #region private API

    void Movement()
    {
        //smooth between previous speed to new speed, for movement animation
        Vector3 localVelocity = Direction.WorldToInverseLocalDirection(rb.velocity, transform.rotation).normalized;
        localVelocity = Vector3.Lerp(previousSpeed, localVelocity, Time.deltaTime * smooth);

        previousSpeed = localVelocity;

        //set animator
        anim.SetFloat("Horizontal", localVelocity.x);
        anim.SetFloat("Vertical", localVelocity.z);
    }

    void Jump()
    {
        //jump if rigidbody is going up and player is grounded
        if (rb.velocity.y > 0.5f && isGrounded)
        {
            anim.SetTrigger("Jump");
            isGrounded = false;
        }

        //check if falling or grounded
        if(rb.velocity.y <= 0 && isGrounded == false)
        {
            isGrounded = true;
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
