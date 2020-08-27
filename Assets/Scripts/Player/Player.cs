using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class Player : StateMachine, IDamage
{
    #region variables

    [Header("Player")]
    public float maxHealth = 100;
    [Header("Player States")]
    public MovingState movingState;
    public FightState fightState;
    public AttackState attackState;
    [Header("Camera")]
    public CameraBaseControl cameraControl;
    [Header("Check Ground")]
    [SerializeField] Vector3 center = Vector3.zero;
    [SerializeField] Vector3 size = Vector3.one;
    [Header("Radius Lock Enemy")]
    [SerializeField] float radius = 20;

    //check in a box, if hit something other than the player
    public bool isGrounded => Physics.OverlapBox(transform.position + center, size / 2, transform.rotation, CreateLayer.LayerAllExcept("Player"), QueryTriggerInteraction.Ignore).Length > 0;

    //for animations
    public System.Action OnJump;
    public System.Action<bool> OnSwitchFight;
    public System.Action<bool> OnAttack;
    public System.Action OnEndAttack;
    public System.Action OnDead;

    [Header("Debug")]
    [SerializeField] float currentHealth;

    Enemy enemy;

    #endregion

    void Awake()
    {
        //set default values
        currentHealth = maxHealth;

        //set default camera
        cameraControl.StartDefault(Camera.main.transform, transform);

        //set state
        SetState(movingState);
    }

    void Update()
    {
        //attacco

        state?.Execution();
    }

    private void OnDrawGizmosSelected()
    {
        //draw check ground
        Gizmos.color = Color.red; 

        //matrix to use transform.rotation
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(center, size);

        //draw sphere to find nearest enemy
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, radius);
    }

    #region public API

    public void ApplyDamage(float damage)
    {
        //apply damage
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            OnDead?.Invoke();
        }
    }

    public Enemy GetEnemy()
    {
        //if not enemy
        if (enemy == null)
        {
            //find nearest enemy
            Collider[] enemies = Physics.OverlapSphere(transform.position, radius, CreateLayer.LayerOnly("Enemy"), QueryTriggerInteraction.Ignore);
            enemy = Utility.FindNearest(enemies, transform.position).GetComponentInParent<Enemy>();
        }

        return enemy;
    }

    #endregion
}
