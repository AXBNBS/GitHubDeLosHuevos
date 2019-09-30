
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectText : MonoBehaviour
{
    [SerializeField] private string[] text;
    private PlayerInteraction playerInt;


    // Start is called before the first frame update.
    private void Start ()
    {
        playerInt = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction> ();
    }


    // Update is called once per frame.
    private void Update ()
    {
        
    }


    // If the player gets close to the interactable object, we'll send him/her the text that will appear if he/she decides to interact with the object.
    private void OnTriggerEnter (Collider other)
    {
        if (other.tag == "Player")
        {
            playerInt.text = this.text;
        }
    }


    // If the player gets far from the interactable object, we'll remove the text reference we had previously sent.
    private void OnTriggerExit (Collider other)
    {
        if (other.tag == "Player")
        {
            playerInt.text = null;
        }
    }
}