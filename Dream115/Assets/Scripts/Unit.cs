
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Unit : MonoBehaviour
{
    public Transform target;

    [SerializeField] private float speed;
    private Vector3[] path;
    private int targetIndex;


    private void Update ()
    {
        if (target == null)
        {
            StopCoroutine ("FollowPath");
        }
        else 
        {
            PathRequestManager.RequestPath (this.transform.position, target.position, OnPathFound);
        }
        /*if (target != null) 
        {
            PathRequestManager.RequestPath (this.transform.position, target.position, OnPathFound);
        }*/
    }


    public void OnPathFound (Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful == true)
        {
            path = newPath;
            targetIndex = 0;

            StopCoroutine ("FollowPath");
            StartCoroutine ("FollowPath");
        }
    }


    IEnumerator FollowPath ()
    {
        if (path != null && path.Length > 0) 
        {
            Vector3 currentWaypoint = path[0];

            while (true)
            {
                if (this.transform.position == currentWaypoint)
                {
                    targetIndex += 1;

                    if (targetIndex >= path.Length)
                    {
                        //StopCoroutine ("FollowPath");
                        yield break;
                    }

                    currentWaypoint = path[targetIndex];
                }
                transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);

                print(currentWaypoint);
                yield return null;
            }
        }
    }
}