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
        [Tooltip("After this time, check if do another attack or end combo")] 
        public float durationAttack;
        [Tooltip("Damage for this attack")] 
        public float damage;
    }

    #endregion

    [Tooltip("List of attacks for this combo")] 
    [SerializeField] AttackStruct[] attacks = default;

    int attackIndex;
    bool goToNextAttack;

    public AttackState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Enter()
    {
        yield return base.Enter();

        //foreach attack in the list
        for (int i = 0; i < attacks.Length; i++)
        {
            //reset
            attackIndex = i;
            goToNextAttack = false;

            //do damage
            DoDamage();

            //then wait end of attack
            yield return new WaitForSeconds(attacks[attackIndex].durationAttack);

            //check if really ended or go to next attack
            if (CheckEndCombo())
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

        //attack
        InputForNextAttack(Input.GetButtonDown("Fire1"));
    }

    #region attack

    void DoDamage()
    {
        //TODO 
        //check for the entire animation if hit something
        Object.FindObjectOfType<Enemy>().GetComponent<IDamage>().ApplyDamage(attacks[attackIndex].damage);
    }

    void InputForNextAttack(bool inputAttack)
    {
        //if press input && there is another attack, set to go to next attack
        if (inputAttack && attackIndex < attacks.Length -1)
        {
            goToNextAttack = true;
        }
    }

    bool CheckEndCombo()
    {
        //if clicked for next attack, go to next attack
        if (goToNextAttack)
        {
            //animation attack (combo sequence)
            player.OnAttack?.Invoke(false);

            return false;
        }

        //else end combo
        return true;
    }

    #endregion

    void EndAttack()
    {
        //come back to fight state
        player.SetState(player.fightState);
        player.OnEndAttack?.Invoke();
    }
}
