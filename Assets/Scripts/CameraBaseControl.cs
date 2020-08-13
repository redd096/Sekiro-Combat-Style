﻿namespace redd096
{
    using System.Collections;
    using UnityEngine;

    [System.Serializable]
    public class CameraBaseControl
    {
        [Header("Important")]
        [SerializeField] Vector3 cameraOffset;
        [SerializeField] bool firstPerson = true;

        [Header("Smooth")]
        [SerializeField] float smoothPosition = 50;
        [SerializeField] float smoothRotation = 15;

        [Header("Sensitivity")]
        [SerializeField] float sensitivityX = 200;
        [SerializeField] float sensitivityY = 200;

        [Header("Clamp X")]
        [SerializeField] float minX = -360f;
        [SerializeField] float maxX = 360f;

        [Header("Clamp Y")]
        [SerializeField] float minY = -60f;
        [SerializeField] float maxY = 60f;

        float rotX, rotY;
        Transform cam;
        Transform player;

        #region IMPORTANT

        /// <summary>
        /// Set references and default rotation
        /// </summary>
        public void StartDefault(Transform cam, Transform player, bool setDefault = true)
        {
            //set references
            this.cam = cam;
            this.player = player;

            if (setDefault)
            {
                //set rotX and rotY
                SetDefaultRotation();

                //set position and rotation
                SetPositionImmediatly();
                SetRotationImmediatly();
            }
        }

        /// <summary>
        /// Make the camera follow the player
        /// </summary>
        public void UpdateCameraPosition()
        {
            if (firstPerson)
            {
                //use player for local rotation, because camera rotate also on X axis (Mouse Y)
                cam.position = Vector3.Slerp(cam.position, player.position + Direction.WorldToLocalDirection(cameraOffset, player.rotation), Time.deltaTime * smoothPosition);
            }
            else
            {
                //you can use camera rotation if you want the camera to move on top and bottom of the player, like 3rd person
                cam.position = Vector3.Slerp(cam.position, player.position + Direction.WorldToLocalDirection(cameraOffset, cam.rotation), Time.deltaTime * smoothPosition);
            }
        }

        /// <summary>
        /// Rotate the camera by input
        /// </summary>
        public void UpdateRotation(float inputX, float inputY)
        {
            //get the rotation we want
            Vector3 camEuler;
            Vector3 playerEuler;
            GetRotations(inputX, inputY, out camEuler, out playerEuler);

            //from vector3 to quaternion
            Quaternion playerRotation;
            Quaternion camRotation;
            FromEulerToRotation(camEuler, playerEuler, out camRotation, out playerRotation);

            //do rotation
            DoRotation(camRotation, playerRotation);
            //DoRotation(camEuler, playerEuler);   //euler -> no smooth
        }

        #endregion

        #region private API

        #region set immediatly

        void SetPositionImmediatly()
        {
            //set camera position
            cam.position = player.position + Direction.WorldToLocalDirection(cameraOffset, player.rotation);
        }

        void SetRotationImmediatly()
        {
            //set camera and player rotation
            cam.rotation = Quaternion.Euler(-rotY, rotX, 0);
            player.rotation = Quaternion.Euler(0, rotX, 0);
        }

        #endregion

        #region rotation

        void GetRotations(float inputX, float inputY, out Vector3 camEuler, out Vector3 playerEuler)
        {
            //we use float, so we can clamp easy
            rotX += inputX * sensitivityX * Time.deltaTime;
            rotY += inputY * sensitivityY * Time.deltaTime;

            rotX = Angle.ClampAngle(rotX, minX, maxX);
            rotY = Angle.ClampAngle(rotY, minY, maxY);

            //the rotation we want for cam and player, on world space
            camEuler = new Vector3(-rotY, rotX, 0);
            playerEuler = new Vector3(0, rotX, 0);
        }

        void FromEulerToRotation(Vector3 camEuler, Vector3 playerEuler, out Quaternion camRotation, out Quaternion playerRotation)
        {
            //from vector3 to quaternion
            camRotation = Quaternion.Euler(camEuler);
            playerRotation = Quaternion.Euler(playerEuler);
        }

        void DoRotation(Quaternion camRotation, Quaternion playerRotation)
        {
            //set rotations
            cam.rotation = Quaternion.Slerp(cam.rotation, camRotation, Time.deltaTime * smoothRotation);
            player.rotation = Quaternion.Slerp(player.rotation, playerRotation, Time.deltaTime * smoothRotation);
        }

        void DoRotation(Vector3 camEuler, Vector3 playerEuler)
        {
            //set rotations - no smooth, cause there is a problem with gimble lock
            cam.eulerAngles = camEuler;
            player.eulerAngles = playerEuler;
        }

        #endregion

        #endregion

        #region public API

        #region set rotation

        /// <summary>
        /// Set default rotation based on player and camera current rotation
        /// </summary>
        public void SetDefaultRotation()
        {
            rotX = player.eulerAngles.y;
            rotY = -cam.eulerAngles.x;
        }

        /// <summary>
        /// Set player and camera rotation
        /// </summary>
        public void SetRotation(Vector3 rotation)
        {
            rotX = rotation.y;
            rotY = -rotation.x;
        }

        /// <summary>
        /// Set player and camera rotation
        /// </summary>
        public void SetRotation(Quaternion rotation)
        {
            Vector3 euler = rotation.eulerAngles;

            rotX = euler.y;
            rotY = -euler.x;
        }

        #endregion

        #region add rotation

        /// <summary>
        /// Add rotation to camera and player, like you moved the mouse
        /// </summary>
        public void AddRotation(float addX, float addY)
        {
            //we add the values
            float rotationX = rotX + addX;
            float rotationY = rotY + addY;

            //maybe we need negative values, like -90 instead of 270, for example with clamp from -90 to 90
            rotationX = Angle.NegativeAngle(rotationX, minX, maxX);
            rotationY = Angle.NegativeAngle(rotationY, minY, maxY);

            //final set
            rotX = rotationX;
            rotY = rotationY;
        }

        /// <summary>
        /// Add smooth rotation to camera and player, like you moved the mouse
        /// </summary>
        public IEnumerator Smooth_AddRotation(float addX, float addY, float durationAnimation)
        {
            //we use local variable, so can't be modified from player during animation
            float startX = rotX;
            float startY = rotY;
            float rotationX = rotX + addX;
            float rotationY = rotY + addY;

            //maybe we need negative values, like -90 instead of 270, for example with clamp from -90 to 90
            rotationX = Angle.NegativeAngle(rotationX, minX, maxX);
            rotationY = Angle.NegativeAngle(rotationY, minY, maxY);

            //animation
            float delta = 0;
            while(delta < 1)
            {
                delta += Time.deltaTime / durationAnimation;

                rotX = Mathf.Lerp(startX, rotationX, delta);
                rotY = Mathf.Lerp(startY, rotationY, delta);

                yield return null;
            }

            //final set
            rotX = rotationX;
            rotY = rotationY;
        }

        #endregion

        #endregion
    }
}