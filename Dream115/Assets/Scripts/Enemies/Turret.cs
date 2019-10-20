using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{

    enum state { PATROL, DETECTED };

    state actualState;

    float turnSpeed = 2.0f;

    public Transform target;
    Transform auxTarget;

    float viewRadius;
    float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public LayerMask rotationPoint;

    public Light light;

    public Transform player;

    public bool shooterEnemy; //Variable que indicará si la torreta solo vigila o dispara

    public Enemy[] enemies; //Cada enemigo guarda toda la lista de enemigos para llamarles si ve al personaje

    public GameObject shot; //Objeto que se disparara
    public Transform shotSpawn; //Spawn del disparo

    private float fireRate = 3f; //Rate de disparo para que no este continuamente disparando
    private float nextFire = 0f; //Tiempo que falta para el siguiente disparo
    private SpriteRenderer[] minimapIcons;


    // Start is called before the first frame update
    void Start()
    {
        actualState = state.PATROL;
        transform.LookAt (new Vector3 (target.position.x, transform.position.y, target.position.z));

        viewRadius = 30f;
        viewAngle = 50f;
        light = light.GetComponent<Light>();
        auxTarget = target;
        minimapIcons = this.gameObject.GetComponentsInChildren<SpriteRenderer> ();
    }


    // Update is called once per frame
    void Update ()
    {
        FindVisibleTargets ();
        Move ();

        if (actualState == state.DETECTED)
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


    private void FindVisibleTargets()
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
                    actualState = state.DETECTED;//Si ve al personaje pasa a estado de persecucion
                    turnSpeed = 3.0f;
                    return;
                }
            }
        }

        if (actualState == state.DETECTED)
        {
            actualState = state.PATROL;//Si no ve al personaje ni investiga una señal sigue patrullando
            turnSpeed = 2.0f;
        }
    }

    private void Move()
    {
        Vector3 lookDirection = new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z).normalized;

        var targetRotation = Quaternion.LookRotation(lookDirection).eulerAngles;

        Quaternion targetRotationOnlyY = Quaternion.Euler(transform.rotation.eulerAngles.x, targetRotation.y, transform.rotation.eulerAngles.z);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationOnlyY, turnSpeed * Time.deltaTime);

        float distToTarget = Vector3.Distance(transform.position, target.position);
        if (Physics.Raycast(transform.position, transform.forward, distToTarget, rotationPoint) && actualState==state.PATROL)
        {
            target = target.gameObject.GetComponent<Waypoint>().nextPoint;
            auxTarget = target;
        }

        if (actualState == state.PATROL)
        {
            light.color = Color.blue;
            target = auxTarget;
        }
        else
        {
            light.color = Color.red;
            target = player;

            if (Vector3.Distance(transform.position, target.position) <= viewRadius && Time.time > nextFire) //Comprueba si hay alguien en rango de tiro
            {
               //animator.SetTrigger("Shoot");
                nextFire = Time.time + fireRate; //Hace que no ejecute otro disparo hasta pasado un tiempo
                Fire(); //Dispara
            }
        }
    }

    private void Fire() //Disparo
    {
        Instantiate(shot, shotSpawn.position, shotSpawn.rotation); //Instancia el tiro
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

        Gizmos.DrawRay(transform.position, transform.forward * 100);
    }
}
