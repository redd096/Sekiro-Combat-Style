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

    void Awake()
    {
        //get references
        player = GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        //be sure to see only layer 0
        anim.SetLayerWeight(1, 0);
        anim.SetLayerWeight(2, 0);

        //set events
        AddEvents();
    }

    void Update()
    {
        //movement
        Movement();
        Jump();
        Falling();
    }

    private void OnDestroy()
    {
        //remove events
        RemoveEvents();
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

    #region events

    void AddEvents()
    {
        //set events
        player.OnSwitchFight = OnSwitchFight;
        player.OnAttack = OnAttack;
        player.OnEndAttack = OnEndAttack;
        player.OnDead = OnDead;
    }

    void RemoveEvents()
    {
        //remove events
        player.OnSwitchFight = null;
        player.OnAttack = null;
        player.OnEndAttack = null;
        player.OnDead = null;
    }

    void OnSwitchFight(bool fightState)
    {
        //switch from unarmed to sword
        anim.SetBool("FightState", fightState);
        anim.SetTrigger("SwitchFight");
    }

    void OnAttack(bool isFirstAttack)
    {
        if(isFirstAttack)
        {
            anim.SetLayerWeight(1, 1);
        }

        //set attack
        anim.SetBool("IsFirstAttack", isFirstAttack);
        anim.SetTrigger("Attack");
    }

    void OnEndAttack()
    {
        anim.SetLayerWeight(1, 0);
    }

    void OnDead()
    {
        //set dead
        anim.SetBool("Dead", true);
    }

    #endregion

    #endregion
}
