using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    [Header("Enemy")]
    [SerializeField] float distanceAttack = 1.5f;
    [SerializeField] AttackStruct currentAttack = default;

    NavMeshAgent nav;
    Transform player;

    [SerializeField] float delayAttack = 2;
    float timer;

    bool stunned;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        player = GameManager.instance.player?.transform;

        //add events
        AddEvents();

        //equip sword
        if (player)
            OnSwitchFight?.Invoke(true);
    }

    void Update()
    {
        //do only if there is player
        if (player == null)
            return;

        Movement();
        Attack();
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    #region private API

    #region events

    void AddEvents()
    {
        OnStartStun += StartStun;
        OnEndStun += EndStun;
        OnDead += Die;
    }

    void RemoveEvents()
    {
        OnStartStun -= StartStun;
        OnEndStun -= EndStun;
        OnDead -= Die;
    }

    void StartStun()
    {
        stunned = true;
    }

    void EndStun()
    {
        stunned = false;
    }

    void Die()
    {
        //stop update and stop navmesh
        enabled = false;
        StopMovement();
    }

    #endregion

    #region movement

    void Movement()
    {
        //if not in attack range, move to player - else don't move
        if (CheckInRange() == false)
            nav.SetDestination(player.position);
        else
            StopMovement();
    }

    void StopMovement()
    {
        //stop navmesh
        nav.SetDestination(transform.position);
    }

    #endregion

    #region attack

    void Attack()
    {
        //if player in range attack
        if(CheckInRange() && Time.time > timer)
        {
            //create layer to hit only enemy and player, and be sure to ignore self
            int layer = CreateLayer.LayerOnly(new string[] { "Enemy", "Player" });
            IDamage self = transform.GetComponent<IDamage>();

            //start weapon attack
            weapon.Attack(currentAttack.timePrepareAttack, currentAttack.durationAttack, currentAttack.damage, layer, self);

            OnAttack?.Invoke(true);
            timer = Time.time + delayAttack;

            Invoke("EndAttack", delayAttack);
        }
    }

    void EndAttack()
    {
        OnEndAttack?.Invoke();
    }

    #endregion

    #region general

    bool CheckInRange()
    {
        //check player distance
        return Vector3.Distance(transform.position, player.position) <= distanceAttack;
    }

    #endregion

    #endregion

    #region public API

    public override void ApplyDamage(IDamage instigator, float damage)
    {
        //if stunned, instant dead
        if (stunned)
        {
            KillSelf();
            return;
        }

        //else normal damage
        base.ApplyDamage(instigator, damage);
    }

    #endregion
}
