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
    [Tooltip("Radius lock target")]
    [SerializeField] float radiusFindTarget = 20;

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
    [Tooltip("Duration animation of deflect")]
    [SerializeField] float durationDeflectMove = 0.5f;

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

    Coroutine rechargeDefense_Coroutine;

    Transform target;

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

    protected virtual void OnDrawGizmosSelected()
    {
        //draw sphere to find nearest target
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, radiusFindTarget);
    }

    protected void KillSelf()
    {
        DamageHealth(currentHealth);
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
            StopDefend();

            //deflect
            Deflect(instigator);

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

    #region deflect and stun

    void Deflect(IDamage instigator)
    {
        //be sure to stop attack animation if running
        OnEndAttack?.Invoke();

        //deflect attack
        instigator.AttackGetDeflected();
        OnDeflectingAttack?.Invoke();

        //wait, then come back to old state
        SetWaitState(durationDeflectMove);
    }

    void Stun()
    {
        //be sure to stop attack animation if running
        OnEndAttack?.Invoke();

        OnStartStun?.Invoke();

        //wait, then come back to old state
        SetWaitState(timeStunned, OnEndStun);
    }

    protected virtual void SetWaitState(float timeToWait, System.Action func = null, bool nullState = false)
    {
        State nextState = nullState ? null : state;

        //wait, then go to next state
        SetState(new WaitState(this, timeToWait, nextState, func));
    }

    #endregion

    #endregion

    #region public API

    #region IDamage

    public virtual void ApplyDamage(IDamage instigator, float damage)
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

    #endregion

    #region defend

    public void StartDefend()
    {
        //return if already defending
        if (isDefending)
            return;

        //if defense is not broken
        if(defenseIsBroken == false)
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

    #region target

    public Transform GetTarget(string layerName)
    {
        //if not enemy
        if (target == null)
        {
            int layer = CreateLayer.LayerOnly(layerName); ;

            //find nearest enemy
            Collider[] targets = Physics.OverlapSphere(transform.position, radiusFindTarget, layer, QueryTriggerInteraction.Ignore);
            target = Utility.FindNearest(targets, transform.position)?.transform;
        }

        return target;
    }

    #endregion

    #endregion
}
