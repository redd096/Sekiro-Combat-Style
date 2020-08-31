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
        //defense (before of all, so it can be suppressed when change state)
        Defend(Input.GetButton("Fire2"));

        base.Execution();

        //lock cam to enemy
        LookEnemy();

        //attack
        Attack(Input.GetButtonDown("Fire1"));
    }

    void Defend(bool inputDefend)
    {
        //if press input, start defend
        if (inputDefend)
        {
            player.StartDefend();
        }
        //else, stop defend
        else
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

    protected override void SwitchFight(bool inputSwitch)
    {
        //if press input, come back to moving state
        if (inputSwitch)
        {
            player.SetState(player.movingState);

            //remove defense
            player.StopDefend();

            //animation switch fight
            player.OnSwitchFight?.Invoke(false);
        }
    }
}
