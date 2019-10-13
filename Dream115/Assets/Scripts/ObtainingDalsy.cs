
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObtainingDalsy : MonoBehaviour
{    
    // Start is called before the first frame update.
    private void Start ()
    {
    }


    // Update is called once per frame.
    private void Update ()
    {
    }


    private void OnTriggerEnter (Collider col)
    {
        if (col.gameObject.tag.Equals ("Player") == true)
        {
            ObtainDalsy ();
        }
    }


    private void ObtainDalsy ()
    {
        PlayerInteraction.Instance.DalsyCatched ();

        //PlayerStatsController player = ; //playerisinvisible
        Destroy (this.gameObject);
    }
}