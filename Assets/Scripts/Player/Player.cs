using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class Player : Character
{
    #region variables

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
    public bool IsGrounded => Physics.OverlapBox(transform.position + center, size / 2, transform.rotation, CreateLayer.LayerAllExcept("Player"), QueryTriggerInteraction.Ignore).Length > 0;

    Enemy enemy;

    #endregion

    void Start()
    {
        //set default camera
        cameraControl.StartDefault(Camera.main.transform, transform);

        //set state
        SetState(movingState);

        //add events
        AddEvents();
    }

    void Update()
    {
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

    private void OnDestroy()
    {
        //remove events
        RemoveEvents();
    }

    #region events

    void AddEvents()
    {
        OnStartStun += StartStun;
        OnEndStun += EndStun;
        OnDead += Die;
    }

    void RemoveEvents()
    {
        OnStartStun -= StartStun;
        OnEndStun -= EndStun;
        OnDead -= Die;
    }

    void StartStun()
    {
        //cambia stato del giocatore, in uno che sta buono finché non finisce lo stun
    }

    void EndStun()
    {
        //ritorna al fight state 
        //(lo stun parte solo se si è in difesa e ci si è rotto lo scudo... in futuro potrebbe partire in attack state se ci deflettono, ma sempre in fight state si torna)
    }

    void Die()
    {
        //deve entrare in uno stato di morte
        //ovvero uno stato in cui deve smettere di muoversi, attaccare, ecc... e può solo cadere se non si trovava a terra (playerState.StopMovement())

        //nel caso decidessi di riportarlo in vita, l'animazione di revive c'è, lo si fa tornare allo state precedente ed è a posto
    }

    #endregion

    #region public API

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
