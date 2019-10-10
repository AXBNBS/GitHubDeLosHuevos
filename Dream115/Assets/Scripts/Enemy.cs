using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float speed = 5.0f;
    float turnSpeed = 4.0f;
    public Transform target;
    Transform auxTarget;

    enum state { PATROL, ALERT, CHASE }; // Control de estados para cuando persiga al personaje

    state actualState;

    float viewRadius;
    float viewAngle;
    float viewRadiusShoot; //Distancia donde dispararía el enemigo

    public bool shooterEnemy; //Variable que indicará si el enemigo es de lejos o de cerca

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

    // LLEVAR COSES D'ACÍ AL FINAL
    public bool backToRoute;

    [SerializeField] private int distance, angle;
    [SerializeField] private Transform[] raycastOrigins;
    private RaycastHit hitInfoOld, hitInfoNew;
    private bool rightObstacle, leftObstacle, xAxis, positiveSign;
    private float limit, colliderLimit;


    // Start is called before the first frame update
    void Start ()
    {
        //this.transform.position = target.position;
        actualState = state.PATROL;
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

        viewRadius = 30f;
        viewAngle = 120f;
        viewRadiusShoot = viewRadius / 1.5f; //Distancia donde dispararía el enemigo

        light = light.GetComponent<Light>();
        auxTarget = target;

        animator = this.GetComponentInChildren<Animator>();
        raycastOrigins = new Transform[2];

        Transform children = this.GetComponentInChildren<Transform>();
        int index = 0;

        foreach (Transform t in children)
        {
            if (t.tag == "RaycastOrigin")
            {
                raycastOrigins[index] = t;
                index += 1;
            }
        }
        rightObstacle = false;
        leftObstacle = false;
        backToRoute = false;
        colliderLimit = this.gameObject.GetComponent<CapsuleCollider>().radius + 1;
    }


    // Update is called once per frame.
    private void Update()
    {
        FindVisibleTargets();
        FollowRoute();
        Move();
    }


    private void Move ()
    {
        transform.Translate (new Vector3 (0, 0, speed * Time.deltaTime));
        Vector3 lookDirection = new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z).normalized;

        var targetRotation = Quaternion.LookRotation(lookDirection).eulerAngles;

        if (actualState != state.PATROL || backToRoute == true)
        {
            if (Physics.Raycast (raycastOrigins[0].position, raycastOrigins[0].forward, out hitInfoNew, distance, obstacleMask, QueryTriggerInteraction.Collide) == true)
            {
                targetRotation.y -= angle;
                //rightObstacle = true;
                print("Right");
            }

            if (Physics.Raycast (raycastOrigins[1].position, raycastOrigins[1].forward, out hitInfoNew, distance, obstacleMask, QueryTriggerInteraction.Collide) == true)
            {
                targetRotation.y += angle;
                //leftObstacle = true;
                print("Left");
            }
        }

        Quaternion targetRotationOnlyY = Quaternion.Euler(transform.rotation.eulerAngles.x, targetRotation.y, transform.rotation.eulerAngles.z);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationOnlyY, turnSpeed * Time.deltaTime);

        if (actualState == state.CHASE)
        {
            light.color = Color.red;
            actualState = state.CHASE;
            target = player;
            foreach (Enemy enemy in enemies)//Avisa a todos los enemigos para que persigan al jugador
            {
                enemy.light.color = Color.red;
                enemy.actualState = state.CHASE;
                enemy.target = player;
            }
            if (shooterEnemy && Vector3.Distance(transform.position, target.position) <= viewRadiusShoot && Time.time > nextFire) //Comprueba si hay alguien en rango de tiro
            {
                nextFire = Time.time + fireRate; //Hace que no ejecute otro disparo hasta pasado un tiempo
                Fire(); //Dispara
            }

            if (Vector3.Distance(transform.position, target.position) < stoppingDistance)//Comprueba si esta lo suficientemente cerca del personaje para parar
            {
                animator.SetFloat("Speed", 0f);//Para de andar
                speed = 0;//
            }
            else
            {
                animator.SetFloat("Speed", 12f);//Sigue persiguiendo al personaje
                speed = 5.0f;
            }
        }

        else if (actualState == state.ALERT)
        {
            light.color = Color.yellow;
            if (Vector3.Distance(transform.position, target.position) < 1.0f)
            {
                Destroy(target.gameObject);
                actualState = state.PATROL;
                backToRoute = true;
                target = auxTarget;
                foreach (Enemy enemy in enemies)
                {
                    if (enemy.actualState != state.PATROL)
                    {
                        enemy.actualState = state.PATROL;
                        enemy.backToRoute = true;
                        enemy.target = enemy.auxTarget;
                    }
                }
            }
        }
        else
        {
            light.color = Color.blue;
            target = auxTarget;
            animator.SetFloat("Speed", 1f);
            speed = 5.0f;
        }
    }


    private void FollowRoute ()
    {
        if (this.actualState == state.PATROL)
        {
            if (Vector3.Distance (this.transform.position, target.transform.position) <= 1)
            {
                backToRoute = false;
                target = target.gameObject.GetComponent<Waypoint>().nextPoint;
                auxTarget = target;
            }
        }
    }


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

        Gizmos.color = Color.yellow;

        Gizmos.DrawRay (raycastOrigins[0].position, (raycastOrigins[0].forward + raycastOrigins[0].right).normalized * distance);
        Gizmos.DrawRay (raycastOrigins[1].position, (raycastOrigins[1].forward - raycastOrigins[1].right).normalized * distance);
        //Gizmos.DrawRay (raycastOrigins[0].position, raycastOrigins[0].right * distance / 3);
        //Gizmos.DrawRay (raycastOrigins[1].position, -raycastOrigins[1].right * distance / 3);

        Gizmos.color = Color.blue; //Cambio el color del gizmo para diferenciar distancia de visionado y de disparo
        Gizmos.DrawWireSphere(transform.position, viewRadiusShoot); //Dibujo de distancia de disparo
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
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask) && !PlayerStats.Instance.playerInvisible) //Si no es invisible 
                {
                    actualState = state.CHASE; //Si ve al personaje pasa a estado de persecucion
                    return;
                }
            }
        }
        if (actualState != state.ALERT)
        {
            actualState = state.PATROL;//Si no ve al personaje ni investiga una señal sigue patrullando
            //backToRoute = true;
        }
    }


    private void Fire () //Disparo
    {
        Instantiate(shot, shotSpawn.position, shotSpawn.rotation); //Instancia el tiro
    }

    public void checkAlert (Transform position)//El personaje llama a esta funcion de los enemigos que iran a investigar la posicion desde la que se ha mandado la señal
    {
        if (actualState == state.ALERT)
        {
            Destroy(target.gameObject);
        }
        actualState = state.ALERT;
        target = position;
        foreach (Enemy enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < 10)
            {
                enemy.actualState = state.ALERT;
                enemy.target = position;
            }
        }
    }
}


    /* Checks the obstacle just hit by the raycast in order to determine the point where there will be no wall next to the character.
    private void DefineLimits ()
    {
        if (xAxis == true)
        {
            Vector3 positiveLimit = hitInfoNew.transform.position + new Vector3 (hitInfoNew.collider.bounds.extents.x, 0, 0);
            Vector3 negativeLimit = hitInfoNew.transform.position - new Vector3 (hitInfoNew.collider.bounds.extents.x, 0, 0);

            if (Vector3.Angle (this.transform.forward, positiveLimit - this.transform.position) <
                Vector3.Angle (this.transform.forward, negativeLimit - this.transform.position))
            {
                limit = positiveLimit.x;
            }
            else
            {
                limit = negativeLimit.x;
            }
        }
        else
        {
            Vector3 positiveLimit = hitInfoNew.transform.position + new Vector3 (0, 0, hitInfoNew.collider.bounds.extents.z);
            Vector3 negativeLimit = hitInfoNew.transform.position - new Vector3 (0, 0, hitInfoNew.collider.bounds.extents.z);

            if (Vector3.Angle (this.transform.forward, positiveLimit - this.transform.position) <
                Vector3.Angle (this.transform.forward, negativeLimit - this.transform.position))
            {
                limit = positiveLimit.z;
            }
            else
            {
                limit = negativeLimit.z;
            }
        }
        hitInfoOld = hitInfoNew;
    }


            if (rightObstacle == true ||
            Physics.Raycast(raycastOrigins[0].position, raycastOrigins[0].forward, out hitInfoNew, distance, obstacleMask, QueryTriggerInteraction.Collide) == true)
        {
            targetRotation.y -= angle;
            rightObstacle = true;
            print("Obstacle on the right");

    /*if (hitInfoNew.transform != hitInfoOld.transform)
    {
        xAxis = hitInfoNew.collider.bounds.extents.x > hitInfoNew.collider.bounds.extents.z;

        DefineLimits ();
    }

    if (xAxis == true)
    {
        if (Mathf.Sign (limit) == -1 && (this.transform.position.x + colliderLimit) < limit || 
            Mathf.Sign (limit) == +1 && (this.transform.position.x - colliderLimit) > limit)
        {
            rightObstacle = false;
        }
    }
    else
    {
        if (Mathf.Sign (limit) == -1 && (this.transform.position.z + colliderLimit) < limit ||
            Mathf.Sign (limit) == +1 && (this.transform.position.z - colliderLimit) > limit)
        {
            rightObstacle = false;
        }
    }*/
        /*else
        {
            if (leftObstacle == true ||
                Physics.Raycast(raycastOrigins[1].position, raycastOrigins[1].forward, out hitInfoNew, distance, obstacleMask, QueryTriggerInteraction.Collide) == true)
            {
                targetRotation.y += angle;
                leftObstacle = true;
                print("Obstacle on the left");

                /*if (hitInfoNew.transform != hitInfoOld.transform)
                {
                    xAxis = hitInfoNew.collider.bounds.extents.x > hitInfoNew.collider.bounds.extents.z;

                    DefineLimits ();
                }

                if (xAxis == true)
                {
                    if (Mathf.Sign (limit) == -1 && (this.transform.position.x + colliderLimit) < limit ||
                        Mathf.Sign (limit) == +1 && (this.transform.position.x - colliderLimit) > limit)
                    {
                        leftObstacle = false;
                    }
                }
                else
                {
                    if (Mathf.Sign (limit) == -1 && (this.transform.position.z + colliderLimit) < limit ||
                        Mathf.Sign (limit) == +1 && (this.transform.position.z - colliderLimit) > limit)
                    {
                        leftObstacle = false;
                    }
                }
            }
        }*/