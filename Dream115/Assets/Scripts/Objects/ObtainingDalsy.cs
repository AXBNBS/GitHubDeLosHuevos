
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObtainingDalsy : MonoBehaviour
{    
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