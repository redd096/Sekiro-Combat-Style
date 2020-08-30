using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class CharacterGraphics : MonoBehaviour
{
    [Header("Smooth")]
    [Tooltip("Smooth, used for movement animations")] 
    [SerializeField] float smoothMovement = 5;
    [Tooltip("Time to set weight of the attack layer")] 
    [SerializeField] float durationBlendAttack = 0.5f;
    [Tooltip("Time to set weight of the defend layer")]
    [SerializeField] float durationBlendDefend = 0.2f;
    [Header("Weapon")]
    [Tooltip("Duration of the weapon lerp. Used when change from hand to holster or viceversa")]
    [SerializeField] float durationLerpWeaponPosition = 0.2f;
    [Tooltip("Time to wait before change parent, when weapon go from holster to hand")]
    [SerializeField] float timeBeforeGrab = 0.3f;
    [Tooltip("Time to wait before change parent, when weapon go from hand to holster")]
    [SerializeField] float timeBeforeRelease = 0.3f;
    [Tooltip("Player hand used to grab weapon")]
    [SerializeField] Transform hand = default;
    [Tooltip("Player holster used to put weapon")]
    [SerializeField] Transform holster = default;

    Character character;
    Animator anim;
    Rigidbody rb;

    Vector3 previousSpeed;

    Coroutine blendAttack_Coroutine;
    Coroutine blendDefend_Coroutine;
    Coroutine blendHit_Coroutine;

    protected virtual void Awake()
    {
        //get references
        character = GetComponent<Character>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        //be sure to see only layer 0
        anim.SetLayerWeight(1, 0);
        anim.SetLayerWeight(2, 0);

        //get weapon and do switch fight animation
        OnSwitchFight(anim.GetBool("FightState"));

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

    protected virtual Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    void Movement()
    {
        //smooth between previous speed to new speed, for movement animation
        Vector3 localVelocity = Direction.WorldToInverseLocalDirection(GetVelocity(), transform.rotation).normalized;
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
        if (GetVelocity().y < -0.5f)
            anim.SetBool("Falling", true);
        else
            anim.SetBool("Falling", false);
    }

    #endregion

    #region events

    void AddEvents()
    {
        //set events
        character.OnJump = OnJump;
        character.OnSwitchFight = OnSwitchFight;
        character.OnAttack = OnAttack;
        character.OnEndAttack = OnEndAttack;
        character.OnStartDefend = OnStartDefend;
        character.OnEndDefend = OnEndDefend;
        character.OnDeflectingAttack = OnDeflectingAttack;

        character.OnStartStun += OnStartStun;
        character.OnEndStun += OnEndStun;
        character.OnDead += OnDead;
}

    void RemoveEvents()
    {
        //remove events
        character.OnJump = null;
        character.OnSwitchFight = null;
        character.OnAttack = null;
        character.OnEndAttack = null;
        character.OnStartDefend = null;
        character.OnEndDefend = null;
        character.OnDeflectingAttack = null;

        character.OnStartStun -= OnStartStun;
        character.OnEndStun -= OnEndStun;
        character.OnDead -= OnDead;
    }

    void OnJump()
    {
        //jump animation
        anim.SetTrigger("Jump");
    }

    void OnSwitchFight(bool goToFightState)
    {
        //switch from unarmed to sword
        anim.SetBool("FightState", goToFightState);
        anim.SetTrigger("SwitchFight");

        //change weapon position
        WeaponChangePosition(goToFightState);
    }

    void OnAttack(bool isFirstAttack)
    {
        //if first attack, start blend layer attack
        if(isFirstAttack)
        {
            if (blendAttack_Coroutine != null)
                StopCoroutine(blendAttack_Coroutine);

            blendAttack_Coroutine = StartCoroutine(BlendLayer(1, durationBlendAttack, true));
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

        blendAttack_Coroutine = StartCoroutine(BlendLayer(1, durationBlendAttack, false));
    }

    void OnStartDefend()
    {
        //start blend layer defend
        if (blendDefend_Coroutine != null)
            StopCoroutine(blendDefend_Coroutine);

        blendDefend_Coroutine = StartCoroutine(BlendLayer(2, durationBlendDefend, true));
    }

    void OnEndDefend()
    {
        //end defend, blend layer to 0
        if (blendDefend_Coroutine != null)
            StopCoroutine(blendDefend_Coroutine);

        blendDefend_Coroutine = StartCoroutine(BlendLayer(2, durationBlendDefend, false));
    }

    void OnDeflectingAttack()
    {
        //deflect attack
        anim.SetTrigger("Deflect");
    }

    void OnStartStun()
    {
        //start stun
        anim.SetBool("Stunned", true);
        anim.SetTrigger("Stun");
    }

    void OnEndStun()
    {
        //end stun
        anim.SetBool("Stunned", false);
    }

    void OnDead()
    {
        //set dead
        anim.SetBool("Dead", true);
        anim.SetTrigger("Die");
    }

    #endregion

    #region general

    IEnumerator BlendLayer(int layerIndex, float durationBlend, bool visible)
    {
        //set start
        float delta = 0;
        float startWeight = anim.GetLayerWeight(layerIndex);
        int weight = visible ? 1 : 0;

        //blend
        while (delta < 1)
        {
            delta += Time.deltaTime / durationBlend;

            float newWeight = Mathf.Lerp(startWeight, weight, delta);

            //set layer weight
            anim.SetLayerWeight(layerIndex, newWeight);

            yield return null;
        }
    }

    void WeaponChangePosition(bool goToFightState)
    {
        Transform parent = goToFightState ? hand : holster;
        float timeToWait = goToFightState ? timeBeforeGrab : timeBeforeRelease;

        //change weapon parent
        character.weapon.ChangeParent(parent, timeToWait, durationLerpWeaponPosition);
    }

    #endregion

    #endregion
}
