
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Unit : MonoBehaviour
{
    public Transform target;

    [SerializeField] private float speed;
    private LayerMask obstaclesLayer;
    private Vector3[] path;
    private int targetIndex;
    private Enemy enemy;
    private float colliderLimit;
    private Vector3 lastFramePos;
    //private RaycastHit hit;


    private void Start ()
    {
        obstaclesLayer = LayerMask.GetMask ("obstacleMask");
        enemy = this.gameObject.GetComponent<Enemy> ();
        colliderLimit = this.gameObject.GetComponent<CapsuleCollider>().radius;
        //InvokeRepeating ("GetPath", 0, 1);
    }


    private void Update ()
    {
        if (IsInvoking ("GetPath") == false && enemy.actualState == Enemy.state.CHASE && Vector3.Distance (this.transform.position, target.position) >= colliderLimit) 
        {
            InvokeRepeating ("GetPath", 0, 1);
        }

        if (enemy.actualState == Enemy.state.PATROL) 
        {
            if (enemy.backToPatrol == true) 
            {
                if (this.transform.position == lastFramePos && Physics.Linecast (this.transform.position, this.transform.position + this.transform.forward * 10, obstaclesLayer) == true) 
                {
                    this.transform.Translate (-this.transform.forward);
                    print("pushed");

                    GetPath ();
                }
            }
            else
            {
                StopAllCoroutines ();

                CancelInvoke ("GetPath");
            }
        }
        //print(IsInvoking("GetPath"));
        /*if (target == null)
        {
            StopCoroutine ("FollowPath");
        }
        else 
        {
            PathRequestManager.RequestPath (this.transform.position, target.position, OnPathFound);
        }*/
        /*if (target != null) 
        {
            PathRequestManager.RequestPath (this.transform.position, target.position, OnPathFound);
        }*/
    }


    public void GetPath () 
    {
        PathRequestManager.RequestPath (this.transform.position, target.position, OnPathFound);
        //print("vamos venga");

        if (IsInvoking ("GetPath") == true && (enemy.actualState != Enemy.state.CHASE || Vector3.Distance (this.transform.position, target.position) < colliderLimit))
        {
            CancelInvoke ("GetPath");
        }
    }


    public void OnPathFound (Vector3[] newPath, bool pathSuccessful)
    {
        //print("owo");
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
        //print("wenas");
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
                        //print("yasta");
                        yield break;
                    }

                    currentWaypoint = path[targetIndex];
                }
                lastFramePos = this.transform.position;
                this.transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);

                //print(this.transform.position);
                yield return null;
            }
        }
    }
}