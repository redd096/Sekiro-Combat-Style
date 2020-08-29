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
    [SerializeField] protected float maxHealth = 100;

    [Header("Debug")]
    [SerializeField] protected float currentHealth;

    //for animations
    public System.Action OnJump;
    public System.Action<bool> OnSwitchFight;
    public System.Action<bool> OnAttack;
    public System.Action OnEndAttack;
    public System.Action OnDead;

    Slider healthBar;

    #endregion

    void Awake()
    {
        //set default values
        currentHealth = maxHealth;

        //get health bar
        healthBar = GetComponentInChildren<Slider>();
        UpdateHealthBar();
    }

    #region API

    void UpdateHealthBar()
    {
        //update health bar value
        healthBar.value = currentHealth / maxHealth;
    }

    public void ApplyDamage(float damage)
    {
        //only if not already dead
        if (currentHealth <= 0)
            return;

        //apply damage
        currentHealth -= damage;
        UpdateHealthBar();

        //die
        if (currentHealth <= 0)
        {
            OnDead?.Invoke();
        }
    }

    #endregion
}
