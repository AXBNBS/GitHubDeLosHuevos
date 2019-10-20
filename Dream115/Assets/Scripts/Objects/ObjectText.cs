
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


    // If the player gets close to the interactable object, we'll send him/her the text that will appear when he/she is pi.
    private void OnTriggerEnter (Collider other)
    {
        if (other.tag == "Player")
        {
            playerInt.text = this.text;
            PlayerStats.Instance.RegalosRecogidos += 1;
        }
    }
}