
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraMovement : MonoBehaviour
{
    public bool allowInput;

    [SerializeField] private Transform target, behind;
    [SerializeField] private int movementSpd, centerSpd, clampUp, clampDown, inputSensitivity;
    private float mouseX, mouseY, rotX, rotY;
    private Quaternion localRotation;
    private bool center;
    private Transform camera;

    
    // Start is called before the first frame update.
    private void Start ()
    {
        // ACTIVATE IN FINAL VERSION
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        allowInput = true;
        rotX = this.transform.localRotation.eulerAngles.x;
        rotY = this.transform.localRotation.eulerAngles.y;
        camera = this.transform.GetChild (0);
        print(behind.rotation.eulerAngles);
        print(camera.rotation.eulerAngles);
    }


    // Update is called once per frame.
    private void Update ()
    {
        if (allowInput == false)
        {
            mouseX = 0f;
            mouseY = 0f;
            center = false;
        }
        else
        {
            mouseX = Input.GetAxis ("Mouse X");
            mouseY = Input.GetAxis ("Mouse Y");
            if (Input.GetButtonDown ("CenterCamera") == true)
            {
                center = true;
            }
        }
        if (center == true)
        {
            Vector3 difference = target.position - behind.position;

            rotX = 0f;
            rotY = Mathf.Atan2 (difference.x, difference.z) * Mathf.Rad2Deg;
            localRotation = Quaternion.Euler (rotX, rotY, 0f);
            this.transform.rotation = Quaternion.Lerp (this.transform.rotation, localRotation, centerSpd * Time.deltaTime);

            if (camera.position.y - behind.position.y < 0.05f)
            {
                center = false;
            }
        }
        else
        {
            rotX += mouseY * inputSensitivity * Time.deltaTime;
            rotX = Mathf.Clamp (rotX, clampDown, clampUp);
            rotY += mouseX * inputSensitivity * Time.deltaTime;
            localRotation = Quaternion.Euler (rotX, rotY, 0);
            transform.rotation = localRotation;
        }
    }


    // LateUpdate is called after all Update functions have been called.
    private void LateUpdate ()
    {
        this.transform.position = Vector3.MoveTowards (this.transform.position, target.position, movementSpd * Time.deltaTime);
    }
}