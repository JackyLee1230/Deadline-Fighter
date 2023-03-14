using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class FirstPersonCamera : MonoBehaviour
    {
        // Variables
        public Transform player;
        public float Sensitivity
        {
            get { return sensitivity; }
            set { sensitivity = value; }
        }
        [Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
        [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;
        float cameraVerticalRotation = 0f;
        float cameraHorizontalRotation = 0f;


        // Start is called before the first frame update
        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
         
        // Update is called once per frame
        void Update()
        {
            float xAxis = Input.GetAxis("Mouse X") * sensitivity;
            cameraHorizontalRotation += xAxis;

            float yAxis = Input.GetAxis("Mouse Y") * sensitivity;
            cameraVerticalRotation += yAxis;
            cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -yRotationLimit, yRotationLimit);

            var xQuat = Quaternion.AngleAxis(cameraHorizontalRotation, Vector3.up);
            var yQuat = Quaternion.AngleAxis(cameraVerticalRotation, Vector3.left);

            transform.localRotation = xQuat * yQuat;
        }
    }

}