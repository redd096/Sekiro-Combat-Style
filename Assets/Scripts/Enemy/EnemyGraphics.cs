using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGraphics : CharacterGraphics
{
    NavMeshAgent nav;

    protected override void Awake()
    {
        base.Awake();

        nav = GetComponent<NavMeshAgent>();
    }

    protected override Vector3 GetVelocity()
    {
        //use navmesh instead of rigidbody
        return nav.velocity;
    }
}
