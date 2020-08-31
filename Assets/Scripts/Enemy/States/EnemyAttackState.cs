using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyAttackState : EnemyState
{
    [SerializeField] AttackStruct currentAttack = default;

    Coroutine attack_Coroutine;
    Coroutine slideForward_Coroutine;

    public EnemyAttackState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        attack_Coroutine = enemy.StartCoroutine(Attack_Coroutine());
    }

    public override void Exit()
    {
        base.Exit();

        //be sure to stop coroutines
        if (attack_Coroutine != null)
            enemy.StopCoroutine(attack_Coroutine);

        if (slideForward_Coroutine != null)
            enemy.StopCoroutine(slideForward_Coroutine);
    }

    #region private API

    IEnumerator Attack_Coroutine()
    {
        //effective attack
        SlideForward();
        Attack();

        //then wait end of attack
        yield return new WaitForSeconds(currentAttack.timeBeforeNextAttack);

        //come back to fight state
        EndAttack();
    }

    #region slide

    void SlideForward()
    {
        //start coroutine to slide
        slideForward_Coroutine = enemy.StartCoroutine(SlideForward_Coroutine());
    }

    IEnumerator SlideForward_Coroutine()
    {
        float time = Time.time + currentAttack.durationSlider;

        //slide forward
        while (Time.time < time)
        {
            DoMovement(transform.forward, currentAttack.slideSpeed);

            yield return null;
        }

        //end slide
        StopMovement();
    }

    #endregion

    #region attack

    void Attack()
    {
        //create layer to hit only enemy and player, and be sure to ignore self
        int layer = CreateLayer.LayerOnly(new string[] { "Enemy", "Player" });
        IDamage self = transform.GetComponent<IDamage>();

        //start weapon attack
        enemy.weapon.Attack(currentAttack.timePrepareAttack, currentAttack.durationAttack, currentAttack.damage, layer, self);
    }

    #endregion

    #endregion

    void EndAttack()
    {
        //come back to fight state
        enemy.SetState(enemy.fightState);
        enemy.OnEndAttack?.Invoke();
    }
}
