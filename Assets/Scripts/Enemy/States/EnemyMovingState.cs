using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyMovingState : EnemyState
{
    [SerializeField] float speed = 2f;

    public EnemyMovingState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //stop movement
        StopMovement();
    }

    public override void Execution()
    {
        base.Execution();

        //look if player come in range
        LookForPlayer();
    }

    void LookForPlayer()
    {
        //look if player come in range
        Transform player = enemy.GetTarget("Player");

        //if player found, switch state
        if (player)
            PlayerFound();
    }

    void PlayerFound()
    {
        //if player found, switch state
        enemy.SetState(enemy.fightState);

        enemy.OnSwitchFight?.Invoke(true);
    }
}
