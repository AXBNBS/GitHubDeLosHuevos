using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    float speed = 5.0f;
    public Transform target;
    Transform auxTarget;

    enum state { PATROL, ALERT, CHASE}; // Control de estados para cuando persiga al personaje

    state actualState;

    float viewRadius;
    float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public Light light;

    public Transform player;

    public List<Transform> visibleTargets = new List<Transform>();

    int rayDistance = 10;
    // Start is called before the first frame update
    void Start()
    {
        actualState = state.PATROL;
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

        viewRadius = 30f;
        viewAngle = 120f;

        light = light.GetComponent<Light>();
        auxTarget = target;

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
        var lookDirection = new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z).normalized;
        var targetRotation = Quaternion.LookRotation(lookDirection);

        var hit = new RaycastHit();

        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance))
        {
            if (!(hit.transform.gameObject.layer == obstacleMask))

                Debug.DrawLine(transform.position, hit.point, Color.white);
            else
                Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.green);
            //lookDirection += hit.normal * new Vector3(20.0f,20.0f,20.0f);
        }
        else{
            Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.green);
        }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);

        if (visibleTargets.Count > 0)
        {
            light.color = Color.red;
            actualState = state.CHASE;
            target = player;
        }
        else
        {
            light.color = Color.yellow;
            actualState = state.PATROL;
            target = auxTarget;
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
    }

    private void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for(int i=0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

}
