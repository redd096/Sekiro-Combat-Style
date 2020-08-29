using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : StateMachine, IDamage
{
    #region variables

    [Header("Character")]
    public MeleeWeapon weapon;
    [SerializeField] protected float maxHealth = 100;

    [Header("Debug")]
    [SerializeField] protected float currentHealth;

    //for animations
    public System.Action OnJump;
    public System.Action<bool> OnSwitchFight;
    public System.Action<bool> OnAttack;
    public System.Action OnEndAttack;
    public System.Action OnDead;

    #endregion

    #region public API

    public void ApplyDamage(float damage)
    {
        //only if not already dead
        if (currentHealth <= 0)
            return;

        //apply damage
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            OnDead?.Invoke();
        }
    }

    #endregion
}
