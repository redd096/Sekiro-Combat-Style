using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FightState : MovingState
{
    public FightState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Execution()
    {
        base.Execution();

        //lock cam to enemy
        LookEnemy();

        //defense
        StartDefend(Input.GetButtonDown("Fire2"));
        StopDefend(Input.GetButtonUp("Fire2"));

        //attack
        Attack(Input.GetButtonDown("Fire1"));
    }

    #region private API

    void StartDefend(bool inputDefense)
    {
        //if press input, start defense
        if (inputDefense)
        {
            player.StartDefend();
        }
    }

    void StopDefend(bool inputStopDefense)
    {
        //if release input, stop defense
        if (inputStopDefense)
        {
            player.StopDefend();
        }
    }

    void Attack(bool inputAttack)
    {
        //if press input, attack
        if (inputAttack)
        {
            player.SetState(player.attackState);

            //remove defense
            player.StopDefend();

            //animation first attack
            player.OnAttack?.Invoke(true);
        }
    }

    #endregion

    protected override void SwitchFight(bool inputSwitch)
    {
        //if press input, come back to moving state
        if (inputSwitch)
        {
            player.SetState(player.movingState);

            //animation switch fight
            player.OnSwitchFight?.Invoke(false);
        }
    }
}
