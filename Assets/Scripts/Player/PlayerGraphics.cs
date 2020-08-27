﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class PlayerGraphics : MonoBehaviour
{
    [Tooltip("Smooth, used for movement animations")] [SerializeField] float smoothMovement = 5;
    [Tooltip("Time to set weight of the layer. Smooth used for blend from movement to attack or get hit animations")] [SerializeField] float durationBlendLayer = 0.5f;

    Player player;
    Animator anim;
    Rigidbody rb;

    Vector3 previousSpeed;

    Coroutine blendAttack_Coroutine;
    Coroutine blendHit_Coroutine;

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
        Vector3 newSpeed = Vector3.Lerp(previousSpeed, localVelocity, Time.deltaTime * smoothMovement);

        //save previous speed
        previousSpeed = newSpeed;

        //set animator
        anim.SetFloat("Horizontal", newSpeed.x);
        anim.SetFloat("Vertical", newSpeed.z);
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
        player.OnJump = OnJump;
        player.OnSwitchFight = OnSwitchFight;
        player.OnAttack = OnAttack;
        player.OnEndAttack = OnEndAttack;
        player.OnDead = OnDead;
    }

    void RemoveEvents()
    {
        //remove events
        player.OnJump = null;
        player.OnSwitchFight = null;
        player.OnAttack = null;
        player.OnEndAttack = null;
        player.OnDead = null;
    }

    void OnJump()
    {
        //jump animation
        anim.SetTrigger("Jump");
    }

    void OnSwitchFight(bool goTofightState)
    {
        //switch from unarmed to sword
        anim.SetBool("FightState", goTofightState);
        anim.SetTrigger("SwitchFight");
    }

    void OnAttack(bool isFirstAttack)
    {
        //if first attack, start blend layer attack
        if(isFirstAttack)
        {
            if (blendAttack_Coroutine != null)
                StopCoroutine(blendAttack_Coroutine);

            blendAttack_Coroutine = StartCoroutine(BlendLayer(1, 1));
        }

        //set attack
        anim.SetBool("IsFirstAttack", isFirstAttack);
        anim.SetTrigger("Attack");
    }

    void OnEndAttack()
    {
        //end attack, blend layer to 0
        if (blendAttack_Coroutine != null)
            StopCoroutine(blendAttack_Coroutine);

        blendAttack_Coroutine = StartCoroutine(BlendLayer(1, 0));
    }

    void OnDead()
    {
        //set dead
        anim.SetBool("Dead", true);
    }

    #endregion

    IEnumerator BlendLayer(int layerIndex, float weight)
    {
        //set start
        float delta = 0;
        float startWeight = anim.GetLayerWeight(layerIndex);

        //blend
        while (delta < 1)
        {
            delta += Time.deltaTime / durationBlendLayer;

            float newWeight = Mathf.Lerp(startWeight, weight, delta);

            //set layer weight
            anim.SetLayerWeight(layerIndex, newWeight);

            yield return null;
        }
    }

    #endregion
}
