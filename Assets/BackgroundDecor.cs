using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundDecor : MonoBehaviour
{
    [SerializeField]
    Vector3 moveVector;
    Vector3 rotateVector;
    Rigidbody rb;
    float scaleFactor;

    // Start is called before the first frame update
    void Start()
    {
        
        moveVector = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z);
        rotateVector = new Vector3(transform.right.x, transform.right.y, transform.right.z);
        rb = GetComponent<Rigidbody>();
        rb.AddTorque(rotateVector * 200f);
        rb.AddForce(moveVector * 200f);
        transform.localScale = Vector3.zero;
        scaleFactor = 0.1f;
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.localScale.x < 1f)
        {
            transform.localScale += Vector3.one * Time.deltaTime * scaleFactor;
            if (transform.localScale.x > 1)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
}
