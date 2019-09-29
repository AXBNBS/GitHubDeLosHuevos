﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NewPlayerMovement : MonoBehaviour
{
    private int walkSpd, runSpd, rotationSpd, gravity;
    private float inputH, inputV;
    private Vector3 movement;
    private CharacterController characterCtr;
    private Animator animator;


    // Start is called before the first frame update.
    private void Start ()
    {
        walkSpd = 6;
        runSpd = 9;
        rotationSpd = 10;
        gravity = 8;
        movement = Vector3.zero;
        characterCtr = this.GetComponent<CharacterController> ();
        animator = this.GetComponentInChildren<Animator> ();
    }

    
    // Update is called once per frame.
    private void Update ()
    {
        inputH = Input.GetAxisRaw ("Horizontal");
        inputV = Input.GetAxisRaw ("Vertical");

        if (inputH != 0 || inputV != 0)
        {
            Move (inputH, inputV);
        }
        else
        {
            movement = Vector3.zero;
            movement.y -= gravity * Time.deltaTime;

            characterCtr.Move (movement);
            animator.SetFloat ("Speed", 0f);
        }
    }


    // Whenever the player presses the movement keys, the character will move in the specified direction, change his animation accordingly and quickly rotate in order to face the direction he's walking to.
    private void Move (float h, float v)
    {
        float angle;
        Quaternion rotation;

        Vector3 inputRelativeToCamera = Camera.main.transform.forward * v + Camera.main.transform.right * h;

        movement = inputRelativeToCamera.normalized * Time.deltaTime;

        if (Input.GetButton ("Run") == true)
        {
            movement *= runSpd;

            animator.SetFloat ("Speed", runSpd);

        }
        else
        {
            movement *= walkSpd;

            animator.SetFloat ("Speed", walkSpd);
        }

        movement.y -= gravity * Time.deltaTime;

        characterCtr.Move (movement);

        angle = Mathf.Atan2 (movement.x, movement.z) * Mathf.Rad2Deg;
        rotation = Quaternion.Euler (this.transform.rotation.x, angle, this.transform.rotation.z);
        this.transform.rotation = Quaternion.Lerp (this.transform.rotation, rotation, rotationSpd * Time.deltaTime);
    }
}