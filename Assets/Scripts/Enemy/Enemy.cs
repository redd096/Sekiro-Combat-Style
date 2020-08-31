using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [Header("Enemy")]
    public EnemyMovingState movingState;
    public EnemyFightState fightState;
    public EnemyAttackState attackState;

    bool stunned;

    void Start()
    {
        //set state
        SetState(movingState);

        //add events
        AddEvents();
    }

    private void Update()
    {
        state?.Execution();
    }

    private void OnDestroy()
    {
        //remove events
        RemoveEvents();
    }

    protected override void SetWaitState(float timeToWait, System.Action func = null, bool nullState = false)
    {
        //this function is called only on stun or deflect, so come back always to fight state. Else null if we want a nullState
        State nextState = nullState ? null : fightState;

        //wait, then go to next state
        SetState(new EnemyWaitState(this, timeToWait, nextState, func));
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
        //change to wait state to stop movement, then after few seconds destroy
        SetWaitState(2, new System.Action(() => Destroy(gameObject)), true);

        //spawn new enemy after few seconds
        GameManager.instance.levelManager.SpawnEnemy(4);
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
