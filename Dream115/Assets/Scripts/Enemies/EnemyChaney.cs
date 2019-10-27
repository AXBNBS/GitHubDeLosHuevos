
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyChaney : MonoBehaviour
{

    public float normalMoveSpd, slowMoveSpd, normalTurnSpd, fastTurnSpd;
    public Transform target;
    Transform auxTarget;

    enum state { PATROL, ALERT, CHASE }; // Control de estados para cuando persiga al personaje

    state actualState;

    float viewRadius;
    float viewAngle;
    //float viewRadiusShoot; //Distancia donde dispararía el enemigo

    //public bool shooterEnemy; //Variable que indicará si el enemigo es de lejos o de cerca
    //public bool beltranoide; //Variable que indica si el enemigo es un beltranoide o no

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public Light light;

    public Transform player;

    int rayDistance = 10;

    //public GameObject shot; //Objeto que se disparara
    //public Transform shotSpawn; //Spawn del disparo

    //private float fireRate = 3f; //Rate de disparo para que no este continuamente disparando
    //private float nextFire = 0f; //Tiempo que falta para el siguiente disparo

    private Animator animator; //El animator por si no habia quedado claro

    //public EnemyChaney[] enemies; //Cada enemigo guarda toda la lista de enemigos para llamarles si ve al personaje

    float stoppingDistance = 3.0f;

    //public bool backToPatrol;

    //[SerializeField] private int frontRaysDst, sideRaysDst, dodgeAngle, deviation, currentNoObsItr, targetNoObsItr;
    //[SerializeField] private Transform[] raycastOrigins;
    //[SerializeField] private bool sideObsR, sideObsL, frontObsR, frontObsL, closeObstacle, clearToTarget;
    //private RaycastHit sideObsRInfo, sideObsLInfo, frontObsRInfo, frontObsLInfo;
    private float colliderLimit;
    private bool charging = false; //Para saber si esta cargando o no
    private bool stopped = false; //Para saber si esta parado
    private SpriteRenderer[] minimapIcons;


    // Start is called before the first frame update
    private void Start ()
    {
        //this.transform.position = target.position;
        actualState = state.PATROL;

        transform.LookAt (new Vector3 (target.position.x, transform.position.y, target.position.z));

        viewRadius = 30f;
        viewAngle = 120f;
        //viewRadiusShoot = viewRadius / 1.5f; //Distancia donde dispararía el enemigo

        light = light.GetComponent<Light> ();
        auxTarget = target;

        animator = this.GetComponentInChildren<Animator> ();
        //raycastOrigins = new Transform[2];

        Transform children = this.GetComponentInChildren<Transform> ();
        int index = 0;

        /*foreach (Transform t in children)
        {
            if (t.tag == "RaycastOrigin")
            {
                raycastOrigins[index] = t;
                index += 1;
            }
        }*/
        //backToPatrol = true;
        /*sideObsR = false;
        sideObsL = false;
        frontObsR = false;
        frontObsL = false;
        closeObstacle = false;*/
        colliderLimit = this.gameObject.GetComponent<CapsuleCollider>().radius + 2;
        minimapIcons = this.gameObject.GetComponentsInChildren<SpriteRenderer> ();
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
            //currentYesObsItr = 0;
        }
        else
        {
            currentNoObsItr = 0;
            //currentYesObsItr += 1;
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

        /* Rays that represent the raycasts launched from the enemy's shoulders onwards.
        Gizmos.color = Color.yellow;

        Gizmos.DrawRay (raycastOrigins[0].position, raycastOrigins[0].forward * frontRaysDst);
        Gizmos.DrawRay (raycastOrigins[1].position, raycastOrigins[1].forward * frontRaysDst);
        Gizmos.DrawRay (raycastOrigins[0].position, raycastOrigins[0].right * sideRaysDst);
        Gizmos.DrawRay (raycastOrigins[1].position, -raycastOrigins[1].right * sideRaysDst);

        // Lines drawn to represent the linecasts launched from the enemy's shoulders to its current target.
        Gizmos.color = Color.black;

        Gizmos.DrawLine (raycastOrigins[0].position, target.position);
        Gizmos.DrawLine (raycastOrigins[1].position, target.position);*/

        //Gizmos.color = Color.blue; //Cambio el color del gizmo para diferenciar distancia de visionado y de disparo

        //Gizmos.DrawWireSphere (transform.position, viewRadiusShoot); //Dibujo de distancia de disparo
    }


    private void Move ()
    {
        // The enemy's speed will be drastically reduced if it's close to an obstacle.
        //if (closeObstacle == false)
        //{
            transform.Translate (new Vector3 (0, 0, normalMoveSpd * Time.deltaTime));
        //}
        /*else
        {
            transform.Translate (new Vector3 (0, 0, slowMoveSpd * Time.deltaTime));
        }*/
        
        Vector3 lookDirection = new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z).normalized;
        var targetRotation = Quaternion.LookRotation(lookDirection).eulerAngles;

        /*if (actualState != state.PATROL || backToPatrol == true)
        {
            LookForObstacles ();

            if (closeObstacle == true)
            {
                if (frontObsL == frontObsR)
                {
                    if (frontObsLInfo.distance < frontObsRInfo.distance)
                    {
                        this.transform.Translate (frontObsLInfo.normal * 2 * normalMoveSpd * Time.deltaTime, Space.World);
                    }
                    else
                    {
                        this.transform.Translate (frontObsRInfo.normal * 2 * normalMoveSpd * Time.deltaTime, Space.World);
                    }
                }
                else
                {
                    if (frontObsL == true)
                    {
                        this.transform.Translate (frontObsLInfo.normal * 2 * normalMoveSpd * Time.deltaTime, Space.World);
                    }
                    else
                    {
                        this.transform.Translate (frontObsRInfo.normal * 2 * normalMoveSpd * Time.deltaTime, Space.World);
                    }
                }
            }

            if (frontObsL == true && frontObsR == true && sideObsL == true && sideObsR == true)
            {
                // If the enemy is not able to see a clear path ahead or at its sides, it will go back.
                deviation = 0;
                targetRotation.y += 180;
                //print ("Unable to see a clear path, going back.");
            }
            else
            {
                if (frontObsR == true && frontObsL == true)
                {
                    if (frontObsRInfo.transform != frontObsLInfo.transform)
                    {
                        if (frontObsRInfo.distance > frontObsLInfo.distance)
                        {
                            // We prioritize dodging the obstacle on the left since that's the closest one.
                            ChangeDeviation (true);
                            //print("Dodging front left obstacle.");
                        }
                        else
                        {
                            // We prioritize dodging the obstacle on the right since that's the closest one.
                            ChangeDeviation (false);
                            //print("Dodging front right obstacle.");
                        }
                    }
                    else
                    {
                        if (sideObsL == sideObsR)
                        {
                            if (Vector3.Angle (this.transform.right, frontObsRInfo.normal) < Vector3.Angle (-this.transform.right, frontObsRInfo.normal))
                            {
                                // The enemy turns right since that's the better option taking its current rotation into account.
                                ChangeDeviation (true);
                                //print("Dodging front obstacle by turning right.");
                            }
                            else
                            {
                                // The enemy turns left since that's the better option taking its current rotation into account.
                                ChangeDeviation (false);
                                //print("Dodging front obstacle by turning left.");
                            }
                        }
                        else
                        {
                            if (sideObsR == true)
                            {
                                // The only place where the enemy sees no obstacles is the right side, so it moves towards that direction.
                                ChangeDeviation (false);
                                //print("Moving left to avoid the other obstacles.");
                            }
                            else
                            {
                                // The only place where the enemy sees no obstacles is the right side, so it moves towards that direction.
                                ChangeDeviation (true);
                                //print("Moving right to avoid the other obstacles.");
                            }
                        }
                    }
                }
                else
                {
                    if (frontObsR == true || frontObsL == true)
                    {
                        if (frontObsL == true)
                        {
                            // The enemy avoids hitting the corner in front of it by turning right.
                            ChangeDeviation (true);
                            //print("Avoiding corner by turning right.");
                        }
                        else
                        {
                            // The enemy avoids hitting the corner in front of it by turning left.
                            ChangeDeviation (false);
                            //print("Avoiding corner by turning left.");
                        }
                    }
                    else
                    {
                        //deviation = 0;
                        if (sideObsR == true || sideObsL == true)
                        {
                            if (sideObsL == true)
                            {
                                // The enemy moves parallel to the wall on its left.
                                targetRotation.y = +Vector3.Angle (new Vector3 (sideObsLInfo.normal.z, sideObsLInfo.normal.y, sideObsLInfo.normal.x), this.transform.forward);
                                //print("Moving parallel to the left wall.");
                            }
                            else
                            {
                                // The enemy moves parallel to the wall on its right.
                                targetRotation.y = -Vector3.Angle (new Vector3 (sideObsRInfo.normal.z, sideObsRInfo.normal.y, sideObsRInfo.normal.x), this.transform.forward);
                                //print("Moving parallel to the right wall.");
                            }
                        }
                    }
                }
            }
        }*/
      
        Quaternion targetRotationOnlyY = Quaternion.Euler (this.transform.rotation.eulerAngles.x, targetRotation.y, this.transform.rotation.eulerAngles.z);

        // The enemy will rotate faster if it's close to an obstacle, in order to avoid clipping throught it.
        /*if (closeObstacle == true)
        {
            this.transform.rotation = Quaternion.Slerp (this.transform.rotation, targetRotationOnlyY, fastTurnSpd * Time.deltaTime);
        }
        else
        {*/
            this.transform.rotation = Quaternion.Slerp (this.transform.rotation, targetRotationOnlyY, normalTurnSpd * Time.deltaTime);
        //}

        if ((actualState == state.CHASE || charging == true) && stopped == false) //Si ha visto al personaje o esta cargando
        {
            light.color = Color.red;
            actualState = state.CHASE;
            animator.SetFloat ("Speed", 12f);//Sigue persiguiendo al personaje
            normalMoveSpd = 20;
            /*foreach (EnemyChaney enemy in enemies)//Avisa a todos los enemigos para que persigan al jugador
            {
                enemy.light.color = Color.red;
                enemy.actualState = state.CHASE;
                enemy.target = player;
            }
            if (shooterEnemy && Vector3.Distance (transform.position, target.position) <= viewRadiusShoot && Time.time > nextFire) //Comprueba si hay alguien en rango de tiro
            {
                animator.SetTrigger("Shoot");
                nextFire = Time.time + fireRate; //Hace que no ejecute otro disparo hasta pasado un tiempo
                Fire(); //Dispara
            }

            if (Vector3.Distance(transform.position, target.position) < stoppingDistance)//Comprueba si esta lo suficientemente cerca del personaje para parar
            {
                animator.SetFloat("Speed", 0f);//Para de andar
                normalMoveSpd = 0;
            }
            else
            {
                animator.SetFloat("Speed", 12f);//Sigue persiguiendo al personaje
                normalMoveSpd = 5.0f;
                if (beltranoide) //Si es un beltranoide
                    normalMoveSpd *= 2.5f; //Te persigue a mayor velocidad
            }*/
        }

        else if (actualState == state.ALERT)
        {
            light.color = Color.yellow;

            if (Vector3.Distance (transform.position, target.position) < colliderLimit)
            {
                Destroy (target.gameObject);

                actualState = state.PATROL;
                //backToPatrol = true;
                target = auxTarget;
                /*foreach (EnemyChaney enemy in enemies)
                {
                    if (enemy.actualState != state.PATROL)
                    {
                        enemy.actualState = state.PATROL;
                        enemy.backToPatrol = true;
                        enemy.target = enemy.auxTarget;
                    }
                }*/
            }
        }
        else
        {
            light.color = Color.blue;
            target = auxTarget;
            if (!stopped)
            {
                animator.SetFloat("Speed", 1f);
                normalMoveSpd = 16;
            }
        }
    }


    private void FollowRoute ()
    {
        if (this.actualState == state.PATROL || charging)
        {
            if (Vector3.Distance (this.transform.position, target.transform.position) <= 1)
            {
                actualState = state.PATROL;
                target = target.gameObject.GetComponent<Waypoint>().nextPoint;
                if (charging)
                {
                    animator.SetFloat("Speed", 0f);//Para de andar
                    normalMoveSpd = 0;
                    charging = false;
                    animator.SetTrigger("Stop");
                    stopped = true;
                    StartCoroutine(StopTime());
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
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance (transform.position, target.position);

                if (!Physics.Raycast (transform.position, dirToTarget, distToTarget, obstacleMask) && !PlayerStats.Instance.playerInvisible) //Si no es invisible 
                {
                    actualState = state.CHASE; //Si ve al personaje pasa a estado de persecucion
                    charging = true; //Se pone a cargar
                    return;
                }
            }
        }
        if (actualState == state.CHASE)
        {
            actualState = state.PATROL;//Si no ve al personaje ni investiga una señal sigue patrullando
            //backToPatrol = true;
        }
    }


    /*private void Fire () //Disparo
    {
        Instantiate(shot, shotSpawn.position, shotSpawn.rotation); //Instancia el tiro
    }


    public void checkAlert (Transform position)//El personaje llama a esta funcion de los enemigos que iran a investigar la posicion desde la que se ha mandado la señal
    {
        if (actualState == state.ALERT)
        {
            Destroy (target.gameObject);
        }

        actualState = state.ALERT;
        target = position;
        foreach (EnemyChaney enemy in enemies)
        {
            if (Vector3.Distance (transform.position, enemy.transform.position) < 10)
            {
                enemy.actualState = state.ALERT;
                enemy.target = position;
            }
        }
    }*/


    // First of all, the two linecasts check if there are no obstacles between the enemy and the current target. If that's the case, no further comprovations are required and the enemy proceeds with its route towards the target. However, if obstacles 
    //are present we also need to check the sides by launching 2 other rays which's info might be useful for later.
    /*private void LookForObstacles ()
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
            closeObstacle = (frontObsL == true && frontObsLInfo.distance < frontRaysDst / 5) || (frontObsR == true && frontObsRInfo.distance < frontRaysDst / 5);
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


    IEnumerator StopTime ()
    {
        yield return new WaitForSeconds (1.5f);

        stopped = false;
    }
}