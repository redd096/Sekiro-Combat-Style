using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyFightState : EnemyState
{
    [SerializeField] float speed = 3.5f;
    [SerializeField] float distanceAttack = 1.5f;

    public EnemyFightState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Execution()
    {
        base.Execution();

        //follow player until death
        FollowPlayer();
    }

    #region movement

    void FollowPlayer()
    {
        Transform player = GameManager.instance.player?.transform;

        //if there is no player, come back to moving state
        if (player == null)
        {
            NoPlayer();
            return;
        }

        //if not in attack range, move to it - else attack
        if (CheckInRange(player) == false)
            DoMovement(player.position, speed);
        else
            Attack();
    }

    bool CheckInRange(Transform player)
    {
        //check player distance
        return Vector3.Distance(transform.position, player.position) <= distanceAttack;
    }

    #endregion

    void NoPlayer()
    {
        //if there is no player, come back to moving state
        enemy.SetState(enemy.movingState);

        enemy.OnSwitchFight?.Invoke(false);
    }

    void Attack()
    {
        //if in range, attack the player
        enemy.SetState(enemy.attackState);

        //animation first attack
        enemy.OnAttack?.Invoke(true);
    }
}
