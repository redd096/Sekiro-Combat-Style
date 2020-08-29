using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : StateMachine
{
    [Header("Character")]
    public MeleeWeapon weapon;

    //for animations
    public System.Action OnJump;
    public System.Action<bool> OnSwitchFight;
    public System.Action<bool> OnAttack;
    public System.Action OnEndAttack;
    public System.Action OnDead;
}
