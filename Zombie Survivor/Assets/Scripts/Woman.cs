using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Woman : MonoBehaviour
{
    public float moveSpeed;
    public float turnSpeed;

    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();    
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);

        Vector3 movement = dir.z * transform.forward * moveSpeed * Time.deltaTime;

        rb.MovePosition(transform.position + movement);

        animator.SetFloat("Move", dir.z);

        float turn = dir.x * turnSpeed * Time.deltaTime;

        transform.rotation *= Quaternion.Euler(0, turn, 0);
    }
}
