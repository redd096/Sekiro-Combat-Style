using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    NavMeshAgent nav;
    Transform player;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        player = redd096.GameManager.instance.player?.transform;

        //equip sword
        if (player)
            OnSwitchFight?.Invoke(true);

        AddEvents();
    }

    void Update()
    {
        //follow player
        if (player)
        {
            nav.SetDestination(player.position);
        }
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    #region events

    void AddEvents()
    {
        OnDead += Die;
    }

    void RemoveEvents()
    {
        OnDead -= Die;
    }

    void Die()
    {
        //stop update and stop navmesh
        enabled = false;
        nav.SetDestination(transform.position);
    }

    #endregion
}
