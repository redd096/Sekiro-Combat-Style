using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class Player : MonoBehaviour
{
    [SerializeField] CameraBaseControl cameraControl = default;
    [Header("Movement")]
    [SerializeField] float speed = 4;
    [SerializeField] float jump = 10;
    [Header("Check Ground")]
    [SerializeField] Vector3 center = Vector3.zero;
    [SerializeField] Vector3 size = Vector3.one;
    [Header("Lock Enemy")]
    [SerializeField] float radius = 20;

    Transform cam;
    Rigidbody rb;

    //check in a box, if hit something other than the player
    bool isGrounded => Physics.OverlapBox(transform.position + center, size / 2, transform.rotation, CreateLayer.LayerAllExcept("Player"), QueryTriggerInteraction.Ignore).Length > 0;

    Transform enemy;

    void Awake()
    {
        //get references
        cam = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        
        //set default camera
        cameraControl.StartDefault(cam, transform);
    }

    void Update()
    {
        //attacco
        //inserire lock sul nemico? usare SetRotation del cameraControl

        //movement
        Movement(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        MoveCamera();
        Rotate(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Jump(Input.GetButtonDown("Jump"));

        LockCam(Input.GetKeyDown(KeyCode.Tab));
        SwitchFight(Input.GetButtonDown("Fire3"));
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

    #region private API

    #region movement

    void Movement(float horizontal, float vertical)
    {
        //get direction and current velocity (less y speed)
        Vector3 direction = Direction.WorldToLocalDirection(new Vector3(horizontal, 0, vertical), transform.rotation);
        Vector3 currentVelocity = rb.velocity - new Vector3(0, rb.velocity.y, 0);

        //new velocity with clamp
        Vector3 newVelocity = direction * speed - currentVelocity;
        newVelocity = Vector3.ClampMagnitude(newVelocity, speed);

        //set velocity (only x and y axis)
        rb.AddForce(newVelocity, ForceMode.VelocityChange);
    }

    void MoveCamera()
    {
        cameraControl.UpdateCameraPosition();
    }

    void Rotate(float inputX, float inputY)
    {
        cameraControl.UpdateRotation(inputX, inputY);
    }

    void Jump(bool inputJump)
    {
        //if press to jump and is grounded, jump (y axis)
        if(inputJump && isGrounded)
        {
            rb.AddForce(transform.up * jump, ForceMode.VelocityChange);
        }
    }

    #endregion

    #region lock cam

    void LockCam(bool inputLock)
    {
        //if input lock
        if (inputLock)
        {
            //remove lock
            if (enemy)
            {
                enemy = null;
            }
            //find nearest enemy
            else
            {
                Collider[] enemies = Physics.OverlapSphere(transform.position, radius, CreateLayer.LayerOnly("Enemy"), QueryTriggerInteraction.Ignore);
                enemy = FindNearest(enemies).transform;
            }
        }

        //look enemy
        if (enemy)
        {
            Vector3 lookEnemy = enemy.position - transform.position;
            cameraControl.SetRotation(Quaternion.LookRotation(lookEnemy, transform.up));
        }
    }

    Collider FindNearest(Collider[] list)
    {
        Collider nearest = null;
        float distance = Mathf.Infinity;

        //foreach collider
        foreach (Collider col in list)
        {
            //check nearest
            float newDistance = Vector3.Distance(col.transform.position, transform.position);
            if (newDistance < distance)
            {
                distance = newDistance;
                nearest = col;
            }
        }

        return nearest;
    }

    #endregion

    void SwitchFight(bool inputSwitch)
    {
        if(inputSwitch)
        {
            //quando è a pugni non può combattere, quando è con la spada deve attaccare
        }
    }

    #endregion
}
