using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    float speed = 5.0f;
    public Transform target;

    enum state { PATROL, ALERT, CHASE}; // Control de estados para cuando persiga al personaje

    state actualState;
    // Start is called before the first frame update
    void Start()
    {
        actualState = state.PATROL;
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.actualState == state.PATROL)
        {
            if (other.tag == "Waypoint")
            {
                target = other.gameObject.GetComponent<Waypoint>().nextPoint;
              //  transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            }
        }
    }
}
