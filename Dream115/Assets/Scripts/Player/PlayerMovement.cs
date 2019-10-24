using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float speed = 6;
    float rotSpeed = 160;
    float rot = 0f;
    float gravity = 8;

    Vector3 moveDirection = Vector3.zero;

    CharacterController controller;
    Animator anim;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }
    
    void Update()
    {
        if (controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.W))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    moveDirection = new Vector3(0, 0, 1);
                    moveDirection *= speed * 1.5f;
                    moveDirection = transform.TransformDirection(moveDirection);
                    anim.SetFloat("Speed", speed * 1.5f);
                } else
                {
                    moveDirection = new Vector3(0, 0, 1);
                    moveDirection *= speed;
                    moveDirection = transform.TransformDirection(moveDirection);
                    anim.SetFloat("Speed", speed);
                }
            }

            if (Input.GetKeyUp (KeyCode.W))
            {
                moveDirection = new Vector3(0, 0, 0);
                anim.SetFloat("Speed", 0);
            }
        }
        rot += Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, rot, 0);

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void FixedUpdate()
    {

    }
}
