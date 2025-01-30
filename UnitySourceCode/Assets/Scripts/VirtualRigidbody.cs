using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualRigidbody : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform user;
    Vector3 prevPosition; 
    Vector3 prevAngle; 
    Rigidbody rb;
    void Start()
    {
        prevPosition = user.position;
        prevAngle = user.eulerAngles;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = user.position - prevPosition;
        rb.angularVelocity = (user.eulerAngles - prevAngle)*Mathf.Deg2Rad;
        transform.position = prevPosition;
        transform.eulerAngles = prevAngle;
    }
}
