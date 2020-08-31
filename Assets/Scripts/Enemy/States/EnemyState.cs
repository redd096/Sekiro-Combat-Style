using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyState : State
{
    protected Enemy enemy;
    protected Transform transform;
    protected NavMeshAgent nav;

    public EnemyState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Awake(StateMachine stateMachine)
    {
        base.Awake(stateMachine);

        //get references
        enemy = stateMachine as Enemy;
        transform = enemy.transform;
        nav = enemy.GetComponent<NavMeshAgent>();
    }

    protected void DoMovement(Vector3 destination, float speed)
    {
        //move to destination
        nav.speed = speed;
        nav.SetDestination(destination);
    }

    protected void StopMovement()
    {
        //stop navmesh
        nav.SetDestination(transform.position);
    }
}
