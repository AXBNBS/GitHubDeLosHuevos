
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DetectObstacles : MonoBehaviour
{
    public bool obstacleDetected;

    [SerializeField] private Transform parentCenter;
    private LayerMask obstacleMask;
    private float distance;


    // Start is called before the first frame update.
    private void Start ()
    {
        obstacleMask = LayerMask.GetMask ("obstacleMask");
        distance = Vector3.Distance (parentCenter.position, this.transform.position);
    }


    // Update is called once per frame.
    private void Update ()
    {
        obstacleDetected = Physics.Raycast (parentCenter.position, this.transform.position - parentCenter.position, distance * 2, obstacleMask);
    }


    //
    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay (parentCenter.position, this.transform.position - parentCenter.position);
    }


    /*// The enemy will take an obstacle into account if it's inside the trigger.
    private void OnTriggerStay (Collider other)
    {
        print(other.name);
        if (other.gameObject.layer == obstacleMask.value)
        {
            obstacleDetected = true;
        }
    }


    // If an obstacle exits the trigger area, there's no reason for the enemy to take it into account anymore (if any other obstacle is still present, OnTriggerStay 
    //will take care of setting the value of the boolean back to true).
    private void OnTriggerExit (Collider other)
    {
        if (other.gameObject.layer == obstacleMask.value)
        {
            obstacleDetected = false;
        }
    }*/
}