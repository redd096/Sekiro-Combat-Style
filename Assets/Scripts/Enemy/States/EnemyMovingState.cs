using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyMovingState : EnemyState
{
    [SerializeField] float speed = 2f;
    [SerializeField] float timeBeforeChangeDirection = 5f;

    float timerPatrol;

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

        //walk around
        WalkAround();

        //look if player come in range
        LookForPlayer();
    }

    #region private API

    void WalkAround()
    {
        //every few seconds
        if(Time.time > timerPatrol)
        {
            //choose a destination and move
            DoMovement(LevelManager.RandomPositionOnNavMesh(), speed);

            timerPatrol = Time.time + timeBeforeChangeDirection;
        }
    }

    void LookForPlayer()
    {
        //look if player come in range
        Transform player = enemy.GetTarget("Player");

        //if player found, switch state
        if (player)
            PlayerFound();
    }

    #endregion

    void PlayerFound()
    {
        //if player found, switch state
        enemy.SetState(enemy.fightState);

        //be sure to stop movement
        StopMovement();

        enemy.OnSwitchFight?.Invoke(true);
    }
}
