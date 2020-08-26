using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class PlayerState : State
{
    protected Player player;
    protected Transform transform;
    protected CameraBaseControl cameraControl;

    public PlayerState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Awake(StateMachine stateMachine)
    {
        base.Awake(stateMachine);

        //get references
        player = stateMachine as Player;
        transform = player.transform;
        cameraControl = player.cameraControl;
    }

    public override void Execution()
    {
        base.Execution();

        //camera update
        MoveCamera();
        Rotate(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    #region private API

    void MoveCamera()
    {
        //move camera
        cameraControl.UpdateCameraPosition();
    }

    void Rotate(float inputX, float inputY)
    {
        //rotate player and camera
        cameraControl.UpdateRotation(inputX, inputY);
    }

    protected void LookEnemy()
    {
        //look enemy
        Enemy enemy = player.GetEnemy();

        if (enemy)
        {
            Vector3 lookEnemy = enemy.transform.position - transform.position;
            Debug.Log(lookEnemy.normalized + "- <color=red> " + transform.position + " / " + enemy.transform.position + " </color>");
            cameraControl.SetRotation(lookEnemy);
            //https://answers.unity.com/questions/600577/camera-rotation-around-player-while-following.html
            //questo per far muovere la camera in terza persona che segue il player.
            //Modifica per la prima persona: dovrà seguire il player invece di AngleAxis e dovrà guardare il player.forward + una rotazione sull'asse y (che potrebbe portarmi punto e a capo)
            //Il player invece ha uno script a parte per ruotare. Si divide player e Camera
        }
    }

    #endregion
}
