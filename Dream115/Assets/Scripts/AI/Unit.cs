
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Unit : MonoBehaviour
{
    public Transform target;

    [SerializeField] private float alertSpd, chaseSpd;
    private LayerMask enemiesLay;
    private Vector3[] path;
    private int targetIndex;
    private Enemy enemy;
    private float colliderLimit;
    private PlayerInteraction playerInt;
    private bool wait;


    private void Start ()
    {
        enemiesLay = LayerMask.GetMask ("Enemies");
        enemy = this.gameObject.GetComponent<Enemy> ();
        colliderLimit = this.gameObject.GetComponent<CapsuleCollider>().radius;
        playerInt = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction> ();
        alertSpd = enemy.normalMoveSpd;
        wait = false;
    }


    private void Update ()
    {
        if (IsInvoking ("GetPath") == false && enemy.actualState == Enemy.state.CHASE && ((PlayerStats.Instance.playerInvisible == false && Vector3.Distance (this.transform.position, target.position) >= colliderLimit) || 
            (PlayerStats.Instance.playerInvisible == true && Vector3.Distance (this.transform.position, playerInt.lastKnownPos) >= colliderLimit))) 
        {
            InvokeRepeating ("GetPath", 0, 1);
        }

        if (enemy.backToPatrol == false && enemy.actualState == Enemy.state.PATROL)
        {
            /*if (enemy.backToPatrol == true)
            {
                /*if (Physics.Linecast(this.transform.position, this.transform.position + this.transform.forward * (colliderLimit + 0.4f), out hit, obstaclesLayer) == true)
                {
                    print("pushed");
                    this.transform.Translate (hit.normal * 3);
                    GetPath ();
                }*/
                /*print ("stuck");
                this.transform.Translate (-this.transform.forward);
                if (this.transform.position == lastFramePos && Physics.Linecast (this.transform.position, this.transform.position + this.transform.forward * 10, obstaclesLayer) == true) 
                if (this.transform.position == lastFramePos)
                {
                    /*this.transform.Translate (-this.transform.forward);
                    print("pushed");

                    print("stuck");
                    GetPath ();
                }
            }
            else
            {*/
                StopAllCoroutines ();

                CancelInvoke ("GetPath");
            //}
        }

        wait = enemy.actualState != Enemy.state.PATROL && Physics.Raycast (this.transform.position, this.transform.forward, colliderLimit * 2, enemiesLay, QueryTriggerInteraction.Collide) == true;
    }


    public void GetPath () 
    {
        if (IsInvoking ("GetPath") == true)
        {
            if (PlayerStats.Instance.playerInvisible == true)
            {
                PathRequestManager.RequestPath (this.transform.position, playerInt.lastKnownPos, OnPathFound);

                if (enemy.actualState != Enemy.state.CHASE || Vector3.Distance (this.transform.position, playerInt.lastKnownPos) < colliderLimit)
                {
                    CancelInvoke ("GetPath");
                }
            }
            else 
            {
                PathRequestManager.RequestPath (this.transform.position, target.position, OnPathFound);

                if (enemy.actualState != Enemy.state.CHASE || Vector3.Distance (this.transform.position, target.position) < colliderLimit)
                {
                    CancelInvoke ("GetPath");
                }
            }
        }
        else 
        {
            PathRequestManager.RequestPath (this.transform.position, target.position, OnPathFound);
        }

        /*PathRequestManager.RequestPath (this.transform.position, target.position, OnPathFound);
        //print("vamos venga");

        if (IsInvoking ("GetPath") == true && (enemy.actualState != Enemy.state.CHASE || Vector3.Distance (this.transform.position, target.position) < colliderLimit))
        {
            CancelInvoke ("GetPath");
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
        print("following");
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
                if (wait == false) 
                {
                    if (enemy.actualState == Enemy.state.ALERT)
                    {
                        this.transform.position = Vector3.MoveTowards (this.transform.position, currentWaypoint, alertSpd * Time.deltaTime);
                    }
                    else
                    {
                        this.transform.position = Vector3.MoveTowards (this.transform.position, currentWaypoint, chaseSpd * Time.deltaTime);
                    }
                }

                //print(this.transform.position);
                yield return null;
            }
        }
    }
}