using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : StateMachine, IDamage
{
    #region variables

    [Header("Character")]
    public MeleeWeapon weapon;
    [SerializeField] float maxHealth = 100;

    [Header("Defense")]
    [Tooltip("Character max defense")]
    [SerializeField] float maxDefense = 100;
    [Tooltip("Minimum defense necessary to defend, after defense is broken")]
    [SerializeField] float minimumDefense = 9;
    [Tooltip("Time to wait before start recharging")]
    [SerializeField] float delayRecharge = 2;
    [Tooltip("How much recharge every second")]
    [SerializeField] float rechargeSpeed = 1;
    [Tooltip("Time to deflect incoming attacks")]
    [SerializeField] float timeToDeflect = 0.2f;

    [Header("Stun")]
    [Tooltip("Damage suffered to defense when attack is being deflected")]
    [SerializeField] float getDeflectedDamage = 100;
    [Tooltip("Duration stun after shield is broken")]
    [SerializeField] float timeStunned = 2;

    [Header("UI")]
    [SerializeField] Slider healthBar = default;
    [SerializeField] Slider defenseBar = default;

    [Header("Debug")]
    [SerializeField] float currentHealth;
    [SerializeField] float currentDefense;

    //events only animations
    public System.Action OnJump;
    public System.Action<bool> OnSwitchFight;
    public System.Action<bool> OnAttack;
    public System.Action OnEndAttack;
    public System.Action OnStartDefend;
    public System.Action OnEndDefend;
    public System.Action OnDeflectingAttack;

    //events also for logic
    public System.Action OnStartStun;
    public System.Action OnEndStun;
    public System.Action OnDead;

    bool isDefending;
    float timeStartDefense;
    bool defenseIsBroken;
    bool stunned;

    Coroutine rechargeDefense_Coroutine;
    Coroutine stun_Coroutine;

    #endregion

    void Awake()
    {
        //set default values
        currentHealth = maxHealth;
        currentDefense = maxDefense;

        //update bars
        UpdateHealthBar();
        UpdateDefenseBar();
    }

    #region private API

    #region bars UI

    void UpdateHealthBar()
    {
        //update health bar value
        healthBar.value = currentHealth / maxHealth;
    }

    void UpdateDefenseBar()
    {
        //update defense bar value
        defenseBar.value = currentDefense / maxDefense;
    }

    #endregion

    #region apply damage

    void DamageHealth(float damage)
    {
        //apply damage
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthBar();

        //die
        if (currentHealth <= 0)
        {
            OnDead?.Invoke();
        }
    }

    void DamageDefense(float damage)
    {
        //remove defense
        currentDefense -= damage;
        currentDefense = Mathf.Max(currentDefense, 0);
        UpdateDefenseBar();

        //broke defense
        if (currentDefense <= 0)
        {
            defenseIsBroken = true;
            StopDefend();

            //start stun
            Stun();
        }

        //restart recharge
        if (rechargeDefense_Coroutine != null)
            StopCoroutine(rechargeDefense_Coroutine);

        rechargeDefense_Coroutine = StartCoroutine(RechargeDefense_Coroutine());
    }

    #endregion

    #region defense

    bool TryDamageDefense(IDamage instigator, float damage)
    {
        //only if defending
        if (isDefending == false)
            return false;

        //deflect attack instead of get damage
        if(Time.time < timeStartDefense + timeToDeflect)
        {
            instigator.AttackGetDeflected();
            return true;
        }

        //remove defense
        DamageDefense(damage);

        return true;
    }

    IEnumerator RechargeDefense_Coroutine()
    {
        //wait before recharge
        yield return new WaitForSeconds(delayRecharge);

        //recharge defense
        while(currentDefense < maxDefense)
        {
            currentDefense += rechargeSpeed * Time.deltaTime;
            UpdateDefenseBar();

            //if reached minimum defense, is not broken
            if(defenseIsBroken && currentDefense >= minimumDefense)
            {
                defenseIsBroken = false;
            }

            yield return null;
        }

        //be sure to have max defense
        currentDefense = maxDefense;
    }

    #endregion

    #region stun

    void Stun()
    {
        if (stun_Coroutine != null)
            StopCoroutine(stun_Coroutine);

        //start stun coroutine
        stun_Coroutine = StartCoroutine(Stun_Coroutine());
    }

    IEnumerator Stun_Coroutine()
    {
        //start stun
        stunned = true;
        OnStartStun?.Invoke();

        //wait
        yield return new WaitForSeconds(timeStunned);

        //end stun
        stunned = false;
        OnEndStun?.Invoke();
    }

    #endregion

    #endregion

    #region public API

    public void ApplyDamage(IDamage instigator, float damage)
    {
        //only if not already dead
        if (currentHealth <= 0)
            return;

        //check if remove defense
        if(TryDamageDefense(instigator, damage) == false)
        {
            //else apply damage
            DamageHealth(damage);
        }
    }

    public void AttackGetDeflected()
    {
        //get hit from deflect
        DamageDefense(getDeflectedDamage);
    }

    public void StartDefend()
    {
        //return if already defending
        if (isDefending)
            return;

        //if defense is not broken and character is not stunned
        if(defenseIsBroken == false && stunned == false)
        {
            //start defending
            isDefending = true;
            timeStartDefense = Time.time;

            OnStartDefend?.Invoke();
        }
    }

    public void StopDefend()
    {
        //stop defending
        if (isDefending)
        {
            isDefending = false;

            OnEndDefend?.Invoke();
        }
    }

    #endregion
}
