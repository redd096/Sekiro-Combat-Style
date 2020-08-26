using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamage
{
    void Start()
    {
        
    }

    void Update()
    {

    }

    public void ApplyDamage(float damage)
    {
        Debug.Log("hit " + damage);
    }
}
