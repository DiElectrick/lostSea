using System.Collections;
using UnityEngine;

public class RaftController : MonoBehaviour
{

    [SerializeField] float oarTimeoutBefore;
    [SerializeField] float oarTimeoutAfter;
    [SerializeField] float oarForce;

    [SerializeField] float speed;

    Rigidbody rb;
    float rotationInput;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        speed = rb.linearVelocity.magnitude;
        rb.AddForce(Vector3.back * rb.linearVelocity.magnitude * 0.25f , ForceMode.Acceleration);



    }

  
}
