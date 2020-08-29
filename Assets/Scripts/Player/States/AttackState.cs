using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackState : PlayerState
{
    #region struct

    [System.Serializable]
    public struct AttackStruct
    {
        [Tooltip("After this time, check if player clicked again, so do another attack, or end combo")] 
        public float timeBeforeNextAttack;
        [Tooltip("Time to wait before check if hit something")]
        public float timePrepareAttack;
        [Tooltip("Time to check if hit something")]
        public float durationAttack;
        [Tooltip("Damage for this attack")] 
        public float damage;
        [Tooltip("Speed player slide forward when attack")]
        public float slideSpeed;
        [Tooltip("Time to slide forward when player attack")]
        public float durationSlider;
    }

    #endregion

    [Tooltip("List of attacks for this combo")] 
    [SerializeField] AttackStruct[] attacks = default;

    AttackStruct currentAttack;
    bool goToNextAttack;
    List<IDamage> alreadyHits = new List<IDamage>();

    Coroutine slideForward_Coroutine;
    Coroutine attack_Coroutine;

    public AttackState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Enter()
    {
        yield return base.Enter();

        //foreach attack in the list
        for (int i = 0; i < attacks.Length; i++)
        {
            //reset attack
            currentAttack = attacks[i];
            goToNextAttack = false;
            alreadyHits = new List<IDamage>();

            //stop coroutines
            if (slideForward_Coroutine != null) player.StopCoroutine(slideForward_Coroutine);
            if (attack_Coroutine != null) player.StopCoroutine(attack_Coroutine);

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

    public override void Execution()
    {
        base.Execution();

        //lock cam to enemy
        LookEnemy();

        //check if do another attack
        InputForNextAttack(Input.GetButtonDown("Fire1"));
    }

    #region private API

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
        //start coroutine to attack
        attack_Coroutine = player.StartCoroutine(Attack_Coroutine());
    }

    IEnumerator Attack_Coroutine()
    {
        //wait before check damage
        yield return new WaitForSeconds(currentAttack.timePrepareAttack);

        //reset weapon previous positions
        player.weapon.Attack(true);

        //do damage for all the attack duration
        float time = Time.time + currentAttack.durationAttack;

        while (Time.time < time)
        {
            DoDamage();

            yield return null;
        }
    }

    void DoDamage()
    {
        //get hits this frame
        RaycastHit[] hits = player.weapon.Attack();

        //foreach hit
        foreach (RaycastHit hit in hits)
        {
            //only if hit something
            if (hit.transform == null)
                continue;

            IDamage damageInterface = hit.transform.GetComponentInParent<IDamage>();

            //if can hit
            if (CanHit(damageInterface))
            {
                //add to already hits
                alreadyHits.Add(damageInterface);

                //and do damage
                damageInterface.ApplyDamage(currentAttack.damage);
            }
        }
    }

    bool CanHit(IDamage hit)
    {
        //if can damage
        if(hit != null)
        {
            //check is not self and is not already hit
            bool isNotSelf = hit != player.GetComponent<IDamage>();
            bool notAlreadyHit = !alreadyHits.Contains(hit);

            return isNotSelf && notAlreadyHit;
        }

        return false;
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
