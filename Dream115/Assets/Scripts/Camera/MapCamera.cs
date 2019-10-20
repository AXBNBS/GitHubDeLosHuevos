
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MapCamera : MonoBehaviour
{
    private Transform target;
    
    // Start is called before the first frame update.
    private void Start ()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }


    // Update is called once per frame.
    private void Update ()
    {
        this.transform.position = new Vector3 (target.position.x, this.transform.position.y, target.position.z);
    }
}