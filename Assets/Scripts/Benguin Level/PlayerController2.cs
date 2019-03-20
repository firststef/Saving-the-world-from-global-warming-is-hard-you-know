using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController2 : MonoBehaviour
{
    public Tilemap tilemap;
    private Rigidbody2D rb;
    private Animator animator;
    private bool IsMoving;
    private Vector2 lastMove;
    public float MoveSpeed = 0.2f;
    public int BenguinNumber;

    public GameObject Benguin;
    public GameObject Benguins;
    public GameObject Arrow;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        BenguinNumber = (int)Random.Range(1, 4);
        for(int i=0; i<BenguinNumber; i++)
        {
            GameObject benguin = Instantiate(Benguin, new Vector3((int)Random.Range(-10, 11), (int)Random.Range(-10, 11), 0), new Quaternion(0, 0, 0, 0), Benguins.transform);
            GameObject arrow = Instantiate(Arrow, gameObject.transform);
            arrow.GetComponent<TargetIndicator>().Target = benguin.transform;
        }
    }

    void FixedUpdate()
    {
        IsMoving = false;

        if ((Input.GetAxisRaw("Vertical") > 0.5 || Input.GetAxisRaw("Vertical") < -0.5) || (Input.GetAxisRaw("Horizontal") > 0.5 || Input.GetAxisRaw("Horizontal") < -0.5))
        {
            IsMoving = true;
            lastMove.y = Input.GetAxisRaw("Vertical");
            lastMove.x = Input.GetAxisRaw("Horizontal");
            rb.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * MoveSpeed, Input.GetAxisRaw("Vertical") * MoveSpeed));
        }
        

        animator.SetBool("IsMoving", IsMoving);
        animator.SetFloat("MoveY", Input.GetAxisRaw("Vertical"));
        animator.SetFloat("MoveX", Input.GetAxisRaw("Horizontal"));
        animator.SetFloat("LastMoveX", lastMove.x);
        animator.SetFloat("LastMoveY", lastMove.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hello");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Hello2");
        Vector3Int pos = tilemap.WorldToCell(GameObject.Find("Player").transform.position);
        pos.z = 0;
        tilemap.SetTile(pos, null);
    }
}
