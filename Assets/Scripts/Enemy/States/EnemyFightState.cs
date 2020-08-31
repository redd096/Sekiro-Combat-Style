using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyFightState : EnemyState
{
    [SerializeField] float speed = 3.5f;
    [Tooltip("Range of attack")]
    [SerializeField] float distanceAttack = 1.5f;
    [Tooltip("Time to wait after enter in this state, after the enemy can move")]
    [SerializeField] float delayCanMove = 1f;
    [Tooltip("Time to wait after enter in this state, after the enemy can defend")]
    [SerializeField] float delayCanDefend = 2f;
    [Tooltip("Time to wait after enter in this state, after the enemy can attack")]
    [SerializeField] float delayCanAttack = 3f;

    Coroutine canMove_Coroutine;
    Coroutine canDefend_Coroutine;
    Coroutine canAttack_Coroutine;

    bool canMove;
    bool canDefend;
    bool canAttack;

    public EnemyFightState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //be sure to wait delays
        canMove = false;
        canDefend = false;
        canAttack = false;

        //start coroutines
        canMove_Coroutine = enemy.StartCoroutine(CanMove_Coroutine());
        canDefend_Coroutine = enemy.StartCoroutine(CanDefend_Coroutine());
        canAttack_Coroutine = enemy.StartCoroutine(CanAttack_Coroutine());
    }

    public override void Execution()
    {
        base.Execution();

        //if defense isn't broken, defend
        TryDefend();

        //follow player until death
        FollowPlayer();
    }

    public override void Exit()
    {
        base.Exit();

        //be sure to not have coroutines running
        if (canMove_Coroutine != null)
            enemy.StopCoroutine(canMove_Coroutine);

        if (canDefend_Coroutine != null)
            enemy.StopCoroutine(canDefend_Coroutine);

        if (canAttack_Coroutine != null)
            enemy.StopCoroutine(canAttack_Coroutine);
    }

    #region private API

    #region coroutines

    IEnumerator CanMove_Coroutine()
    {
        //wait
        yield return new WaitForSeconds(delayCanMove);

        //now can move
        canMove = true;
    }

    IEnumerator CanDefend_Coroutine()
    {
        //wait
        yield return new WaitForSeconds(delayCanDefend);

        //now can defend
        canDefend = true;
    }

    IEnumerator CanAttack_Coroutine()
    {
        //wait
        yield return new WaitForSeconds(delayCanAttack);

        //now can attack
        canAttack = true;
    }

    #endregion

    #region try defend

    void TryDefend()
    {
        //do only if can defend
        if (canDefend == false)
            return;

        //if defense isn't broken, defend
        enemy.StartDefend();
    }

    #endregion

    #region movement

    void FollowPlayer()
    {
        //do only if can move
        if (canMove == false)
            return;

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

    #endregion

    void NoPlayer()
    {
        //if there is no player, come back to moving state
        enemy.SetState(enemy.movingState);

        //remove defense
        enemy.StopDefend();

        //animation switch fight
        enemy.OnSwitchFight?.Invoke(false);
    }

    void Attack()
    {
        //do only if can attack
        if (canAttack == false)
            return;

        //if in range, attack the player
        enemy.SetState(enemy.attackState);

        //remove defense
        enemy.StopDefend();

        //animation first attack
        enemy.OnAttack?.Invoke(true);
    }
}
