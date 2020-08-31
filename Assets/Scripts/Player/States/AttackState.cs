using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackState : PlayerState
{
    [Tooltip("List of attacks for this combo")] 
    [SerializeField] AttackStruct[] attacks = default;

    AttackStruct currentAttack;
    bool goToNextAttack;

    Coroutine combo_Coroutine;
    Coroutine slideForward_Coroutine;

    public AttackState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //start combo coroutine
        combo_Coroutine = player.StartCoroutine(Combo_Coroutine());
    }

    public override void Execution()
    {
        base.Execution();

        //lock cam to enemy
        LookEnemy();

        //check if do another attack
        InputForNextAttack(Input.GetButtonDown("Fire1"));
    }

    public override void Exit()
    {
        base.Exit();

        //be sure to stop coroutines
        if (combo_Coroutine != null)
            player.StopCoroutine(combo_Coroutine);

        if (slideForward_Coroutine != null)
            player.StopCoroutine(slideForward_Coroutine);
    }

    #region private API

    IEnumerator Combo_Coroutine()
    {
        //foreach attack in the list
        for (int i = 0; i < attacks.Length; i++)
        {
            //reset attack
            currentAttack = attacks[i];
            goToNextAttack = false;

            //stop coroutines
            if (slideForward_Coroutine != null) player.StopCoroutine(slideForward_Coroutine);

            //effective attack
            SlideForward();
            Attack();

            //then wait end of attack
            yield return new WaitForSeconds(currentAttack.timeBeforeNextAttack);

            //check if ended combo or go to next attack
            bool lastAttack = i >= attacks.Length - 1;
            if (CheckEndCombo(lastAttack))
                break;
        }

        //come back to fight state
        EndAttack();
    }

    #region slide

    void SlideForward()
    {
        //start coroutine to slide
        slideForward_Coroutine = player.StartCoroutine(SlideForward_Coroutine());
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
        //create layer to hit only enemy, and be sure to ignore self
        int layer = CreateLayer.LayerOnly("Enemy");
        IDamage self = transform.GetComponent<IDamage>();

        //start weapon attack
        player.weapon.Attack(currentAttack.timePrepareAttack, currentAttack.durationAttack, currentAttack.damage, layer, self);
    }

    #endregion

    #region general

    void InputForNextAttack(bool inputAttack)
    {
        //if press input, set to go to next attack
        if (inputAttack)
        {
            goToNextAttack = true;
        }
    }

    bool CheckEndCombo(bool lastAttack)
    {
        //if clicked for next attack, and is not last attack, go to next attack
        if (goToNextAttack && lastAttack == false)
        {
            //animation attack (combo sequence)
            player.OnAttack?.Invoke(false);

            return false;
        }

        //else end combo
        return true;
    }

    #endregion

    #endregion

    void EndAttack()
    {
        //come back to fight state
        player.SetState(player.fightState);
        player.OnEndAttack?.Invoke();
    }
}
