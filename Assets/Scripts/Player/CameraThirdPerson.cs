using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThirdPerson : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] Transform cam = default;
    [SerializeField] Vector3 offset = Vector3.zero;
    [Header("Vars")]
    [SerializeField] float turnSpeed = 4;
    [SerializeField] bool invertX = false;
    [SerializeField] bool invertY = false;
    [Header("Target")]
    [SerializeField] Transform target = default;

    Vector3 position;

    private void Start()
    {
        cam.position = target.position + (target.rotation * offset);
        position = cam.position;
    }

    void LateUpdate()
    {
        //check invert
        int invertHorizontal = invertX ? -1 : 1;
        int invertVertical = invertY ? -1 : 1;

        //input
        position = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed * invertHorizontal, target.up) * position;
        position = Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * turnSpeed * invertVertical, target.right) * position;

        //set position and rotation
        cam.position = target.position + position;
        cam.LookAt(target.position + Vector3.up * offset.y);

        //Enemy enemy = GetComponent<Player>().GetEnemy();
        //Vector3 lookEnemy = enemy.transform.position - target.position;
        //Debug.Log(lookEnemy.normalized + "- <color=red> " + transform.position + " / " + enemy.transform.position + " </color>");
        //cam.rotation = Quaternion.LookRotation(lookEnemy);

        target.rotation = Quaternion.Euler(0, cam.eulerAngles.y, 0);
    }
}
