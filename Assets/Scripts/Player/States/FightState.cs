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

        //attack
        Attack(Input.GetButtonDown("Fire1"));
    }

    void Attack(bool inputAttack)
    {
        //if press input, attack
        if (inputAttack)
        {
            player.SetState(player.attackState);

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

            //animation switch fight
            player.OnSwitchFight?.Invoke(false);
        }
    }
}
