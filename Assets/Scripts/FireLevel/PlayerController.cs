using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    MapManager gameManagerMap;

    public Sprite[] Sprites; //trebuie sa scot odata tileset-ul asta
    public float MoveSpeed = 5f;
    public Slider WaterSlider;
    public Slider WaterLoadSlider;
    public int Water = 0;
    private Vector2 lastMove;
    private bool IsMoving;
    private Rigidbody2D rb;
    private Animator animator;
    public GameObject Water_Bar_Container;
    public int seconds = 0;
    public int FireNumber;
    public GameObject Fire;
    public GameObject FireyBoys;
    public List<Transform> FirePositions;
    private int[] ChosenPos;

    // Start is called before the first frame update
    void Start()
    {
        //Pre-Level
        gameManagerMap = GameObject.Find("GameManagerMap").GetComponent<MapManager>();
        gameManagerMap.player.SetActive(false);

        //Level
        WaterSlider.value = 0;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Sprites = Resources.LoadAll<Sprite>("Tileset");

        FireNumber = Random.Range(1, 6);
        ChosenPos = new int[8];

        for (int i = 0; i < FireNumber; i++)
        {
            while (true)
            {
                int pos = Random.Range(1, 8);
                if (ChosenPos[pos] == 0)
                {
                    Instantiate(Fire, FirePositions[pos].position, new Quaternion(0, 0, 0, 0), FireyBoys.transform);
                    ChosenPos[pos] = 1;
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        IsMoving = false;

        if ((Input.GetAxisRaw("Vertical") > 0.5 || Input.GetAxisRaw("Vertical") < -0.5) || (Input.GetAxisRaw("Horizontal") > 0.5 || Input.GetAxisRaw("Horizontal") < -0.5))
        {
            IsMoving = true;
            lastMove.y = Input.GetAxisRaw("Vertical");
            lastMove.x = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * MoveSpeed, Input.GetAxisRaw("Vertical") * MoveSpeed);
        }

        if (Input.GetAxisRaw("Vertical") < 0.5 && Input.GetAxisRaw("Vertical") > -0.5)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        if (Input.GetAxisRaw("Horizontal") < 0.5 && Input.GetAxisRaw("Horizontal") > -0.5)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

        }

        animator.SetBool("IsMoving", IsMoving);
        animator.SetFloat("MoveY", Input.GetAxisRaw("Vertical"));
        animator.SetFloat("MoveX", Input.GetAxisRaw("Horizontal"));
        animator.SetFloat("LastMoveX", lastMove.x);
        animator.SetFloat("LastMoveY", lastMove.y);


        Water_Bar_Container.transform.position = new Vector3(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y + 0.4f, Water_Bar_Container.transform.position.z);

    }

    void Update()
    {
        if (FireNumber == 0)
        {
            gameManagerMap.completedMiniGame = true;
            gameManagerMap.playingMiniGame = false;
            SceneManager.LoadScene(0);
            gameManagerMap.player.SetActive(true);
            MapManager.dangerPopupsHolder.SetActive(true);
            gameManagerMap.gameProgress.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.name == "WaterSupply")
        {
            if (Water < 4)
            {
                if (seconds > 40)
                {
                    Water++;
                    seconds = 0;
                }

                else seconds++;
            }
        }

        WaterSlider.value = (float)Water / 4;
        WaterLoadSlider.value = (float)seconds / 40;
    }
}
