using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityController : MonoBehaviour
{

    Rigidbody2D rigidbody;
    float Horizontal, Vertical;

    [SerializeField]
    float RunSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        Vertical = Input.GetAxisRaw("Vertical"); // -1 is down
    }

    private void FixedUpdate()
    {
        Vector2 velocity = new Vector2(Horizontal, Vertical).normalized * RunSpeed;

        rigidbody.velocity = velocity;
    }


}

