using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAi : MonoBehaviour
{
    public Vector3[] target;
    public float speed;
    private int current = 0;
    private Rigidbody2D rb;

    private int k;
    private Vector3 pos;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pos = transform.position;
    }

    void Update()
    {
        k++;
        //rb.MoveRotation(k);
        if (transform.position != target[current] && Vector3.Distance(transform.position, target[current]) > 0.05)
        {
            Vector3 pos = Vector3.MoveTowards(transform.position, target[current], speed * Time.deltaTime);
            rb.MovePosition(pos);


        }
        else
        { current = (current + 1) % target.Length; }
    }
}
