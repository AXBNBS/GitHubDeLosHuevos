
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy : MonoBehaviour
{

    public float normalMoveSpd, slowMoveSpd, normalTurnSpd, fastTurnSpd;
    public Transform target;
    Transform auxTarget;

    public enum state { PATROL, ALERT, CHASE }; // Control de estados para cuando persiga al personaje

    public state actualState;

    float viewRadius;
    float viewAngle;
    float viewRadiusShoot; //Distancia donde dispararía el enemigo

    public bool shooterEnemy; //Variable que indicará si el enemigo es de lejos o de cerca
    public bool beltranoide; //Variable que indica si el enemigo es un beltranoide o no

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public Light light;

    public Transform player;

    int rayDistance = 10;

    public GameObject shot; //Objeto que se disparara
    public Transform shotSpawn; //Spawn del disparo

    private float fireRate = 3f; //Rate de disparo para que no este continuamente disparando
    private float nextFire = 0f; //Tiempo que falta para el siguiente disparo

    private Animator animator; //El animator por si no habia quedado claro

    public Enemy[] enemies; //Cada enemigo guarda toda la lista de enemigos para llamarles si ve al personaje

    float stoppingDistance = 3.0f;

    public bool backToPatrol, followingPlayer;
    public Unit unit;

    //[SerializeField] private int frontRaysDst, sideRaysDst, dodgeAngle, deviation, currentNoObsItr, targetNoObsItr;
    //[SerializeField] private Transform[] raycastOrigins;
    //[SerializeField] private bool sideObsR, sideObsL, frontObsR, frontObsL, closeObstacle, clearToTarget;
    //private RaycastHit sideObsRInfo, sideObsLInfo, frontObsRInfo, frontObsLInfo;
    private float colliderLimit;
    private SpriteRenderer[] minimapIcons;
    private bool uniqueWpt;


    // Start is called before the first frame update
    private void Start ()
    {
        //this.transform.position = target.position;
        actualState = state.PATROL;

        transform.LookAt (new Vector3 (target.position.x, transform.position.y, target.position.z));

        viewRadius = 30f;
        viewAngle = 120f;
        viewRadiusShoot = viewRadius / 1.5f; //Distancia donde dispararía el enemigo

        light = light.GetComponent<Light> ();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        auxTarget = target;
        animator = this.GetComponentInChildren<Animator> ();

        /*GameObject[] enemiesObj = GameObject.FindGameObjectsWithTag ("Enemy");

        enemies = new Enemy[enemiesObj.Length];
        for (int i = 0; i < enemies.Length; i += 1) 
        {
            enemies[i] = enemiesObj[i].GetComponent<Enemy> ();
        }*/
        //currentNoObsItr = 0;
        //raycastOrigins = new Transform[2];

        /*BoxCollider[] children = this.GetComponentsInChildren<BoxCollider> ();
        int index = 0;

        foreach (BoxCollider t in children)
        {
            if (t.tag == "RaycastOrigin")
            {
                raycastOrigins[index] = t.transform;
                index += 1;
            }
        }*/
        backToPatrol = false;
        followingPlayer = false;
        /*sideObsR = false;
        sideObsL = false;
        frontObsR = false;
        frontObsL = false;
        closeObstacle = false;*/
        colliderLimit = this.gameObject.GetComponent<CapsuleCollider>().radius;
        //deviation = 0;
        minimapIcons = this.gameObject.GetComponentsInChildren<SpriteRenderer> ();
        unit = this.gameObject.GetComponent<Unit> ();
        uniqueWpt = target.GetComponent<Waypoint>().nextPoint == null;
    }


    // Update is called once per frame.
    private void Update ()
    {
        FindVisibleTargets ();
        FollowRoute ();
        Move ();

        /*if (frontObsR == false && frontObsL == false)
        {
            currentNoObsItr += 1;
        }
        else 
        {
            currentNoObsItr = 0;
        }
        if (currentNoObsItr >= targetNoObsItr) 
        {
            deviation = 0;
        }*/
        if (actualState == state.CHASE)
        {
            minimapIcons[0].enabled = false;
            minimapIcons[1].enabled = true;
        }
        else 
        {
            minimapIcons[0].enabled = true;
            minimapIcons[1].enabled = false;
        }
    }


    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere (transform.position, viewRadius);

        Vector3 viewAngleA = DirFromAngle (-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle (+viewAngle / 2, false);

        Gizmos.DrawLine (transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine (transform.position, transform.position + viewAngleB * viewRadius);

        // Rays that represent the raycasts launched from the enemy's shoulders onwards.
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine (this.transform.position, this.transform.position + this.transform.forward.normalized * 10);

        Gizmos.color = Color.blue; //Cambio el color del gizmo para diferenciar distancia de visionado y de disparo

        Gizmos.DrawWireSphere (transform.position, viewRadiusShoot); //Dibujo de distancia de disparo
    }


    private void Move ()
    {
        if (actualState == state.PATROL && backToPatrol == false) 
        {
            /*float distanceWpt = Vector3.Distance (this.transform.position, target.position);

            if (distanceWpt >= 1) 
            {*/
                this.transform.Translate (new Vector3 (0, 0, normalMoveSpd * Time.deltaTime));
            //}

            /*if (uniqueWpt == true && distanceWpt < 1)
            {
                this.transform.rotation = Quaternion.Slerp (this.transform.rotation, target.rotation, normalTurnSpd * Time.deltaTime);
            }
            else
            {*/
            //}
        }

        // The enemy's speed will be drastically reduced if it's close to an obstacle.
        //if (closeObstacle == false)
        //{

        /*}
        else
        {
            transform.Translate (new Vector3 (0, 0, slowMoveSpd * Time.deltaTime));
        }*/
        //this.transform.Translate (new Vector3 (0, 0, normalMoveSpd * Time.deltaTime));

        /*Vector3 lookDirection = new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z).normalized;
        var targetRotation = Quaternion.LookRotation(lookDirection).eulerAngles;
        Quaternion targetRotationOnlyY = Quaternion.Euler (this.transform.rotation.eulerAngles.x, targetRotation.y, this.transform.rotation.eulerAngles.z);*/

        // The enemy will rotate faster if it's close to an obstacle, in order to avoid clipping throught it.
        /*if (closeObstacle == true)
        {
            this.transform.rotation = Quaternion.Slerp (this.transform.rotation, targetRotationOnlyY, fastTurnSpd * Time.deltaTime);
        }
        else
        {*/
        //this.transform.rotation = Quaternion.Slerp (this.transform.rotation, targetRotationOnlyY, normalTurnSpd * Time.deltaTime);
        //}

        Vector3 lookDirection = new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z).normalized;
        var targetRotation = Quaternion.LookRotation(lookDirection).eulerAngles;
        Quaternion targetRotationOnlyY = Quaternion.Euler (this.transform.rotation.eulerAngles.x, targetRotation.y, this.transform.rotation.eulerAngles.z);

        this.transform.rotation = Quaternion.Slerp (this.transform.rotation, targetRotationOnlyY, normalTurnSpd * Time.deltaTime);

        if (actualState == state.CHASE)
        {
            //light.color = Color.red;
            //actualState = state.CHASE;
            foreach (Enemy enemy in enemies)//Avisa a todos los enemigos para que persigan al jugador
            {
                enemy.light.color = Color.red;
                enemy.actualState = state.CHASE;
                enemy.backToPatrol = false;
                enemy.target = player;
                enemy.unit.target = enemy.target;
            }

            if (shooterEnemy && Vector3.Distance (transform.position, target.position) <= viewRadiusShoot && Time.time > nextFire) //Comprueba si hay alguien en rango de tiro
            {
                animator.SetTrigger ("Shoot");

                nextFire = Time.time + fireRate; //Hace que no ejecute otro disparo hasta pasado un tiempo
                
                Fire (); //Dispara
            }

            if (Vector3.Distance (transform.position, target.position) < colliderLimit)//Comprueba si esta lo suficientemente cerca del personaje para parar
            {
                animator.SetFloat ("Speed", 0f);//Para de andar

                //unit.target = null;
                //normalMoveSpd = 0;//
            }
            else
            {
                animator.SetFloat ("Speed", 12f);//Sigue persiguiendo al personaje

                //unit.target = target;
                if (beltranoide) //Si es un beltranoide
                    normalMoveSpd *= 2.5f; //Te persigue a mayor velocidad
            }
        }
        else if (actualState == state.ALERT)
        {
            light.color = Color.yellow;

            if (Vector3.Distance (transform.position, target.position) < colliderLimit * 2)
            {
                Destroy (target.gameObject);

                /*PatrolAgain ();
                actualState = state.PATROL;
                backToPatrol = true;
                target = auxTarget;*/
                foreach (Enemy enemy in enemies)
                {
                    if (enemy.actualState != state.PATROL)
                    {
                        enemy.PatrolAgain ();
                        /*enemy.actualState = state.PATROL;
                        enemy.backToPatrol = true;
                        enemy.target = enemy.auxTarget;*/
                    }
                }
            }
        }
        else
        {
            light.color = Color.blue;
            target = auxTarget;
            /*if (backToPatrol == false) 
            {
                unit.target = null;
            }*/
            animator.SetFloat ("Speed", 1f);
        }
    }


    private void FollowRoute ()
    {
        if (this.actualState == state.PATROL)
        {
            float distance = Vector3.Distance (this.transform.position, target.transform.position);
            //print(distance);

            if (distance < colliderLimit * 2)
            {
                backToPatrol = false;
                unit.target = null;
                if (uniqueWpt == false) 
                {
                    target = target.gameObject.GetComponent<Waypoint>().nextPoint;
                }
                auxTarget = target;
            }
        }
    }


    public Vector3 DirFromAngle (float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }


    private void FindVisibleTargets ()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i += 1)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle (transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance (transform.position, target.position);

                if (PlayerStats.Instance.playerInvisible == false && Physics.Raycast (transform.position, dirToTarget, distToTarget, obstacleMask) == false) //Si no es invisible y no encontramos obstáculos por el medio.
                {
                    actualState = state.CHASE; //Si ve al personaje pasa a estado de persecución.
                    /*target = player;
                    unit.target = target;
                    backToPatrol = false;*/

                    StopAllCoroutines ();

                    return;
                }
            }
        }
        if (actualState == state.CHASE)
        {
            StartCoroutine ("KeepFollow");
            //actualState = state.PATROL;//Si no ve al personaje ni investiga una señal sigue patrullando
            //backToPatrol = true;
        }
    }


    private void Fire () //Disparo
    {
        Instantiate (shot, shotSpawn.position, shotSpawn.rotation); //Instancia el tiro
    }


    public void checkAlert (Transform position)//El personaje llama a esta funcion de los enemigos que iran a investigar la posicion desde la que se ha mandado la señal
    {
        if (actualState == state.ALERT)
        {
            Destroy (target.gameObject);
        }

        actualState = state.ALERT;
        backToPatrol = false;
        target = position;
        unit.target = target;

        unit.GetPath ();

        foreach (Enemy enemy in enemies)
        {
            if (Vector3.Distance (transform.position, enemy.transform.position) < 10)
            {
                enemy.actualState = state.ALERT;
                enemy.backToPatrol = false;
                enemy.target = position;
                enemy.unit.target = enemy.target;

                enemy.unit.GetPath ();
            }
        }
    }


    /* First of all, the two linecasts check if there are no obstacles between the enemy and the current target. If that's the case, no further comprovations are required and the enemy proceeds with its route towards the target. However, if obstacles 
    //are present we also need to check the sides by launching 2 other rays which's info might be useful for later.
    private void LookForObstacles ()
    {
        clearToTarget = Physics.Linecast (raycastOrigins[0].position, target.position, obstacleMask, QueryTriggerInteraction.Collide) == false && 
            Physics.Linecast (raycastOrigins[1].position, target.position, obstacleMask, QueryTriggerInteraction.Collide) == false;
        if (clearToTarget == true)
        {
            frontObsR = false;
            frontObsL = false;
            sideObsR = false;
            sideObsL = false;
            closeObstacle = false;
            //deviation = 0;
        }
        else
        {
            frontObsR = Physics.Raycast (raycastOrigins[0].position, raycastOrigins[0].forward, out frontObsRInfo, frontRaysDst, obstacleMask, QueryTriggerInteraction.Collide);
            frontObsL = Physics.Raycast (raycastOrigins[1].position, raycastOrigins[1].forward, out frontObsLInfo, frontRaysDst, obstacleMask, QueryTriggerInteraction.Collide);
            sideObsR = Physics.Raycast (raycastOrigins[0].position, raycastOrigins[0].right, out sideObsRInfo, sideRaysDst, obstacleMask, QueryTriggerInteraction.Collide);
            sideObsL = Physics.Raycast (raycastOrigins[1].position, -raycastOrigins[1].right, out sideObsLInfo, sideRaysDst, obstacleMask, QueryTriggerInteraction.Collide);
            /*frontObsR = Physics.Raycast (raycastOrigins[0].position, raycastOrigins[0].forward, out frontObsRInfo, frontRaysDst, obstacleMask, QueryTriggerInteraction.Collide);
            frontObsL = Physics.Raycast (raycastOrigins[1].position, raycastOrigins[1].forward, out frontObsLInfo, frontRaysDst, obstacleMask, QueryTriggerInteraction.Collide);
            sideObsR = Physics.Raycast (raycastOrigins[0].position, raycastOrigins[0].right, out sideObsRInfo, sideRaysDst, obstacleMask, QueryTriggerInteraction.Collide);
            sideObsL = Physics.Raycast (raycastOrigins[1].position, -raycastOrigins[1].right, out sideObsLInfo, sideRaysDst, obstacleMask, QueryTriggerInteraction.Collide);
            closeObstacle = (frontObsR == true && frontObsRInfo.distance < frontRaysDst / 5) || (frontObsL == true && frontObsLInfo.distance < frontRaysDst / 5);
        }
    }*/


    /* We change the value of the deviation we'll add to the rotation of the enemy, we'll also reset the deviation value to 0 if the rotation direction changes suddenly.
    private void ChangeDeviation (bool add)
    {
        if ((Mathf.Sign (deviation) == +1 && add == false) || Mathf.Sign (deviation) == -1 && add == true)
        {
            deviation = 0;
        }
        if (add == true)
        {
            deviation += dodgeAngle;
        }
        else
        {
            deviation -= dodgeAngle;
        }

        deviation = Mathf.Clamp (deviation, -270, +270);
    }*/


    // The enemy goes back to the point it was patrolling to, and we make sure it finds a path to it.
    public void PatrolAgain () 
    {
        StopAllCoroutines ();
        
        RaycastHit hit;

        print(":)");
        backToPatrol = true;
        actualState = state.PATROL;
        target = auxTarget;
        unit.target = target;
        /*if (Physics.Raycast (this.transform.position, this.transform.forward, colliderLimit * 2, obstacleMask) == true) 
        if (Physics.Linecast (this.transform.position, this.transform.position + this.transform.forward.normalized * 10, out hit, obstacleMask) == true)
        {
            print("pushback");
            this.transform.Translate (-hit.normal);
        }*/

        unit.GetPath ();
    }


    // Causes the enemy to keep following the player despite losing sight of him/her.
    IEnumerator KeepFollow ()
    {
        followingPlayer = true;

        yield return new WaitForSeconds (5);

        followingPlayer = false;

        bool playerLost = true;

        foreach (Enemy e in enemies) 
        {
            if (e.followingPlayer == true) 
            {
                playerLost = false;

                break;
            }
        }

        if (playerLost == true) 
        {
            foreach (Enemy e in enemies)
            {
                e.PatrolAgain ();
            }
        }
    }
}