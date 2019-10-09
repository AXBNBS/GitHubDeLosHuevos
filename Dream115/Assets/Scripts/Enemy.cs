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

    enum state { PATROL, ALERT, CHASE}; // Control de estados para cuando persiga al personaje

    state actualState;

    float viewRadius;
    float viewAngle;
    float viewRadiusShoot; //Distancia donde dispararía el enemigo

    public bool shooterEnemy; //Variable que indicará si el enemigo es de lejos o de cerca

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public Light light;

    public Transform player;

    public List<Transform> visibleTargets = new List<Transform>();

    int rayDistance = 10;

    public GameObject shot; //Objeto que se disparara
    public Transform shotSpawn; //Spawn del disparo

    private float fireRate = 3f; //Rate de disparo para que no este continuamente disparando
    private float nextFire = 0f; //Tiempo que falta para el siguiente disparo

    private Animator animator; //El animator por si no habia quedado claro

    public Enemy[] enemies; //Cada enemigo guarda toda la lista de enemigos para llamarles si ve al personaje

    float stoppingDistance = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        actualState = state.PATROL;
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

        viewRadius = 30f;
        viewAngle = 120f;
        viewRadiusShoot = viewRadius / 1.5f; //Distancia donde dispararía el enemigo

        light = light.GetComponent<Light>();
        auxTarget = target;

        animator = this.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        FindVisibleTargets();
        FollowRoute();
        Move();

    }

    private void Move()
    {
        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        Vector3 lookDirection = new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z).normalized;

        var targetRotation = Quaternion.LookRotation(lookDirection);

        Quaternion targetRotationOnlyY = Quaternion.Euler(transform.rotation.eulerAngles.x, targetRotation.eulerAngles.y, transform.eulerAngles.z);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        if (actualState==state.CHASE)
        {
            light.color = Color.red;
            actualState = state.CHASE; 
            target = player;
            foreach( Enemy enemy in enemies)//Avisa a todos los enemigos para que persigan al jugador
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
            if(Vector3.Distance(transform.position, target.position) < 1.0f)
            {
                Destroy(target.gameObject);
                actualState = state.PATROL;
                target = auxTarget;
                foreach (Enemy enemy in enemies)
                {
                   enemy.actualState = state.PATROL;
                   enemy.target = enemy.auxTarget;                 
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

    private void FollowRoute()
    {
        if (this.actualState == state.PATROL)
        {
            if (Vector3.Distance(this.transform.position,target.transform.position)<=1.0)
            {
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

        Gizmos.color = Color.blue; //Cambio el color del gizmo para diferenciar distancia de visionado y de disparo
        Gizmos.DrawWireSphere(transform.position, viewRadiusShoot); //Dibujo de distancia de disparo
    }

    private void FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for(int i=0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask) && !PlayerStats.Instance.playerInvisible) //Si no es invisible 
                {
                    actualState = state.CHASE;//Si ve al personaje pasa a estado de persecucion
                    return;
                }
            }
        }
        if (actualState != state.ALERT)
        {
            actualState = state.PATROL;//Si no ve al personaje ni investiga una señal sigue patrullando
        }
    }

    private void Fire() //Disparo
    {
        Instantiate(shot, shotSpawn.position, shotSpawn.rotation); //Instancia el tiro
    }

    public void checkAlert(Transform position)//El personaje llama a esta funcion de los enemigos que iran a investigar la posicion desde la que se ha mandado la señal
    {
        if (actualState == state.ALERT)
        {
            Destroy(target.gameObject);
        }
        actualState = state.ALERT;
        target = position;
        foreach(Enemy enemy in enemies)
        {
            if(Vector3.Distance(transform.position, enemy.transform.position) < 10)
            {
                enemy.actualState = state.ALERT;
                enemy.target = position;
            }
        }
    }
}
