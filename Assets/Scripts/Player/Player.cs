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
