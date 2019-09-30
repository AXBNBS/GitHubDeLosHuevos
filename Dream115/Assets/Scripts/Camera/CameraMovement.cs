
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraMovement : MonoBehaviour
{
    public bool allowInput;
    [SerializeField] private Transform target;
    [SerializeField] private int speed, clampUp, clampDown, inputSensitivity;
    private float mouseX, mouseY, rotX, rotY;
    private Quaternion localRotation;

    
    // Start is called before the first frame update.
    private void Start ()
    {
        // ACTIVATE IN FINAL VERSION
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        allowInput = true;
        speed = 120;
        clampUp = +80;
        clampDown = -30;
        inputSensitivity = 150;
        rotX = this.transform.localRotation.eulerAngles.x;
        rotY = this.transform.localRotation.eulerAngles.y;
    }


    // Update is called once per frame.
    private void Update ()
    {
        if (allowInput == false)
        {
            mouseX = 0f;
            mouseY = 0f;
        }
        else
        {
            mouseX = Input.GetAxis ("Mouse X");
            mouseY = Input.GetAxis ("Mouse Y");
        }
        rotY += mouseX * inputSensitivity * Time.deltaTime;
        rotX += mouseY * inputSensitivity * Time.deltaTime;
        rotX = Mathf.Clamp (rotX, clampDown, clampUp);
        localRotation = Quaternion.Euler (rotX, rotY, 0);
        transform.rotation = localRotation;
    }


    // LateUpdate is called after all Update functions have been called.
    private void LateUpdate ()
    {
        this.transform.position = Vector3.MoveTowards (this.transform.position, target.position, speed * Time.deltaTime);
    }
}