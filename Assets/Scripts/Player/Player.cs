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

    //check in a box, if hit something other than the player
    public bool IsGrounded => Physics.OverlapBox(transform.position + center, size / 2, transform.rotation, CreateLayer.LayerAllExcept("Player"), QueryTriggerInteraction.Ignore).Length > 0;

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

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        //draw check ground
        Gizmos.color = Color.red; 

        //matrix to use transform.rotation
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(center, size);
    }

    private void OnDestroy()
    {
        //remove events
        RemoveEvents();
    }

    protected override void SetWaitState(float timeToWait, System.Action func = null, bool nullState = false)
    {
        //this function is called only on stun or deflect, so come back always to fight state. Else null if we want a nullState
        State nextState = nullState ? null : fightState;

        //wait, then go to next state
        SetState(new PlayerWaitState(this, timeToWait, nextState, func));
    }

    #region events

    void AddEvents()
    {
        OnDead += Die;
    }

    void RemoveEvents()
    {
        OnDead -= Die;
    }

    void Die()
    {
        //change to wait state to stop movement, then change state to null
        SetWaitState(5, null, true);
    }

    #endregion
}
