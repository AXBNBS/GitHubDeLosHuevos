
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NewPlayerMovement : MonoBehaviour
{
    public bool allowInput;
    public GameObject claim;

    //[SerializeField] private Renderer model;
    //private Material[] materials;
    private int walkSpd, runSpd, rotationSpd, gravity;
    private float inputH, inputV;
    private Vector3 movement;
    private CharacterController characterCtr;
    private Animator animator;
    private PlayerLife playerLife;
    private bool invulnerability = false; //Invulnerabilidad
    private GameObject auxTransform;


    // Start is called before the first frame update.
    private void Start ()
    {
        allowInput = true;
        //materials = model.materials;
        walkSpd = 10;
        runSpd = 16;
        rotationSpd = 10;
        gravity = 8;
        movement = Vector3.zero;
        characterCtr = this.GetComponent<CharacterController> ();
        animator = this.GetComponentInChildren<Animator> ();

        playerLife = this.GetComponent<PlayerLife>();
    }

    
    // Update is called once per frame.
    private void Update ()
    {
        if (allowInput == false)
        {
            inputH = 0f;
            inputV = 0f;
        }
        else
        {
            inputH = Input.GetAxisRaw ("Horizontal");
            inputV = Input.GetAxisRaw ("Vertical");
        }

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

            if (Input.GetKeyDown(KeyCode.T)) //Cuando estes quieto y aprietes F
            {
                animator.SetTrigger("Clap"); //Generas una palmada
                //Que hace sonidos
                CallEnemies();
            }
        }

        if (characterCtr.enabled == false) //Cuando mueres no puedes girar la camara
            rotationSpd = 0;
    }


    private void OnTriggerEnter (Collider collision)
    {
        if (invulnerability == false && collision.gameObject.tag == "Enemy")
        {
            playerLife.TakeDamage (40f);
            invulnerability = true;

            StartCoroutine (InvulnerabilityWaitTime ());
        }
    }


    private void OnTriggerStay (Collider collision)
    {
        if (invulnerability == false && collision.gameObject.tag == "Enemy")
        {
            playerLife.TakeDamage (40f);
            invulnerability = true;
            StartCoroutine (InvulnerabilityWaitTime ());
        }
    }


    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere (transform.position, 200);
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


    private void CallEnemies ()
    {
        Collider[] enemiesinArea = Physics.OverlapSphere(transform.position, 200);

        for (int i = 0; i < enemiesinArea.Length; i++)
        {
            if (enemiesinArea[i].tag == "Enemy")
            {
                auxTransform = Instantiate (claim, transform.position, transform.rotation);
                enemiesinArea[i].SendMessage("checkAlert", auxTransform.transform);
            }
        }
    }


    IEnumerator InvulnerabilityWaitTime ()
    {
        yield return new WaitForSeconds (2f);
        invulnerability = false;
    }
}