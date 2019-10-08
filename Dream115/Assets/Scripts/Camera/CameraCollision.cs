
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraCollision : MonoBehaviour
{
    [SerializeField] private float minimumDst;
    [SerializeField] private int maximumDst, smooth;
    private float distance;
    private Vector3 direction, adjustedDirection, desiredPosition;
    private RaycastHit hit;


    // Awake is always called before any Start function and after every object has been initialized.
    private void Awake ()
    {
        direction = this.transform.localPosition.normalized;
        distance = this.transform.localPosition.magnitude;
    }


    // Update is called once per frame.
    private void Update ()
    {
        desiredPosition = this.transform.parent.TransformPoint (direction * maximumDst);

        if (Physics.Linecast (this.transform.parent.position, desiredPosition, out hit) == true)
        {
            distance = Mathf.Clamp (hit.distance * 0.7f, minimumDst, maximumDst);
        }
        else
        {
            distance = maximumDst;
        }

        this.transform.localPosition = Vector3.Lerp (this.transform.localPosition, direction * distance, Time.deltaTime * smooth);
    }
}