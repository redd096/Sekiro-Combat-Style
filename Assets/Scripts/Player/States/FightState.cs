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
        Defend(Input.GetButton("Fire2"));

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
