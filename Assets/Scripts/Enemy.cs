using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamage
{
    [Header("Enemy")]
    [SerializeField] float maxHealth = 100;

    [Header("Debug")]
    [SerializeField] float currentHealth;

    void Start()
    {
        //set default values
        currentHealth = maxHealth;
    }

    void Update()
    {

    }

    public void ApplyDamage(float damage)
    {
        //apply damage
        currentHealth -= damage;

        Debug.Log(currentHealth);

        if (currentHealth <= 0)
        {
            //Die
            Debug.Log("Dead");
        }
    }
}
