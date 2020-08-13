using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] CameraBaseControl cameraControl;
    [SerializeField] float speed = 4;

    Transform cam;
    Animator anim;
    Rigidbody rb;

    void Awake()
    {
        cam = Camera.main.transform;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        
        cameraControl.StartDefault(cam, transform);
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //estrai o riponi arma
        //attacco
        //inserire lock sul nemico? va a modificare il CameraBaseControl

        Movement(horizontal, vertical);
        MoveCamera();
        Rotate(mouseX, mouseY);

        Animation(horizontal, vertical);
    }

    void Movement(float horizontal, float vertical)
    {
        rb.velocity = Direction.WorldToLocalDirection(new Vector3(horizontal, 0, vertical) * speed, transform.rotation);
    }

    void MoveCamera()
    {
        cameraControl.UpdateCameraPosition();
    }

    void Rotate(float inputX, float inputY)
    {
        cameraControl.UpdateRotation(inputX, inputY);
    }

    void Animation(float horizontal, float vertical)
    {
        anim.SetFloat("Horizontal", horizontal);
        anim.SetFloat("Vertical", vertical);
    }
}
