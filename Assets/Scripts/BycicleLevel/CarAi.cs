using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarAi : MonoBehaviour
{
    public GameObject player;
    public Vector3[] target;
    private int count;
    public float speed;
    private int current = 0;
    private Rigidbody2D rb;

    private Vector3 pos;
    private GameObject lv;
    public bool HitCheckpoint;

    void Awake()
    {
        HitCheckpoint = false;
        lv = GameObject.Find("Levels");
        int rand = (int)Random.Range(1, 4);

        if (rand == 1)
        {
            lv.transform.GetChild(0).gameObject.SetActive(true);
            lv.transform.GetChild(1).gameObject.SetActive(false);
            lv.transform.GetChild(2).gameObject.SetActive(false);

            count = 13;

            target[0] = new Vector3(-1.272f, -0.338f, 0f);
            target[1] = new Vector3(-1.25f, 0.043f, 0f);
            target[2] = new Vector3(-1.142f, 0.212f, 0f);
            target[3] = new Vector3(-0.891f, 0.268f, 0f);
            target[4] = new Vector3(0.626f, 0.216f, 0f);
            target[5] = new Vector3(0.722f, 0.039f, 0f);
            target[6] = new Vector3(0.791f, -0.251f, 0f);
            target[7] = new Vector3(0.704f, -0.598f, 0f);
            target[8] = new Vector3(0.539f, -0.698f, 0f);
            target[9] = new Vector3(-0.133f, -0.754f, 0f);
            target[10] = new Vector3(-0.978f, -0.715f, 0f);
            target[11] = new Vector3(-1.216f, -0.611f, 0f);
            target[12] = new Vector3(-1.272f, -0.425f, 0f);
        }
        else if (rand == 2)
        {
            lv.transform.GetChild(0).gameObject.SetActive(false);
            lv.transform.GetChild(1).gameObject.SetActive(true);
            lv.transform.GetChild(2).gameObject.SetActive(false);

            count = 21;

            target[0] = new Vector3(-1.278f, -0.277f, 0f);
            target[1] = new Vector3(-1.23f, -0.087f, 0f);
            target[2] = new Vector3(-1.108f, 0.064f, 0f);
            target[3] = new Vector3(-0.905f, 0.18f, 0f);
            target[4] = new Vector3(-0.525f, 0.28f, 0f);
            target[5] = new Vector3(0.003f, 0.29f, 0f);
            target[6] = new Vector3(0.377f, 0.193f, 0f);
            target[7] = new Vector3(0.686f, -0.026f, 0f);
            target[8] = new Vector3(0.786f, -0.338f, 0f);
            target[9] = new Vector3(0.709f, -0.631f, 0f);
            target[10] = new Vector3(0.535f, -0.747f, 0f);
            target[11] = new Vector3(0.251f, -0.731f, 0f);
            target[12] = new Vector3(0.164f, -0.448f, 0f);
            target[13] = new Vector3(0.051f, -0.248f, 0f);
            target[14] = new Vector3(-0.126f, -0.145f, 0f);
            target[15] = new Vector3(-0.448f, -0.155f, 0f);
            target[16] = new Vector3(-0.661f, -0.342f, 0f);
            target[17] = new Vector3(-0.742f, -0.674f, 0f);
            target[18] = new Vector3(-0.961f, -0.771f, 0f);
            target[19] = new Vector3(-1.219f, -0.662f, 0f);
            target[20] = new Vector3(-1.271f, -0.42f, 0f);
        }
        else if (rand == 3)
        {
            lv.transform.GetChild(0).gameObject.SetActive(false);
            lv.transform.GetChild(1).gameObject.SetActive(false);
            lv.transform.GetChild(2).gameObject.SetActive(true);

            count = 17;

            target[0] = new Vector3(-1.326f, -0.372f, 0f);
            target[1] = new Vector3(-1.321f, -0.063f, 0f);
            target[2] = new Vector3(-1.115f, 0.149f, 0f);
            target[3] = new Vector3(-0.705f, 0.307f, 0f);
            target[4] = new Vector3(-0.241f, 0.288f, 0f);
            target[5] = new Vector3(0.067f, 0.068f, 0f);
            target[6] = new Vector3(0.346f, 0.216f, 0f);
            target[7] = new Vector3(0.645f, 0.187f, 0f);
            target[8] = new Vector3(0.786f, -0.031f, 0f);
            target[9] = new Vector3(0.792f, -0.457f, 0f);
            target[10] = new Vector3(0.619f, -0.686f, 0f);
            target[11] = new Vector3(0.286f, -0.692f, 0f);
            target[12] = new Vector3(0.076f, -0.547f, 0f);
            target[13] = new Vector3(-0.199f, -0.728f, 0f);
            target[14] = new Vector3(-0.59f, -0.792f, 0f);
            target[15] = new Vector3(-1.089f, -0.658f, 0f);
            target[16] = new Vector3(-1.309f, -0.427f, 0f);
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pos = transform.position;
    }

    void Update()
    {
        if (transform.position != target[current] && Vector3.Distance(transform.position, target[current]) > 0.03)
        {
            pos = Vector3.MoveTowards(transform.position, target[current], speed * Time.deltaTime);
            rb.MovePosition(pos);

            Vector3 dir = target[(current + 1) % count] - transform.position;
            float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
        }
        else
        { current = (current + 1) % count; }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "CheckPoint")
            HitCheckpoint = true;

        if (other.name == "Finish")
        {
            if (HitCheckpoint)
            {
                player.transform.position = new Vector3(-1.317f, -0.28f, 0f);
                player.transform.rotation = new Quaternion(0,0,0,0);
                player.GetComponent<CarMovement>().HitCheckpoint = false;
            }
        }
    }
}
