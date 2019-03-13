using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    MapManager gameManagerMap;
    public Sprite[] Sprites;
    public float MoveSpeed = 5f;
    public Slider WaterSlider;
    public Slider WaterLoadSlider;
    private int Water = 0;
    private Vector2 lastMove;
    private bool IsMoving;
    private Rigidbody2D rb;
    private Animator animator;
    public GameObject Water_Bar_Container;
    public int seconds = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerMap = GameObject.Find("GameManagerMap").GetComponent<MapManager>();
        gameManagerMap.player.SetActive(false);

        WaterSlider.value = 0;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Sprites = Resources.LoadAll<Sprite>("Tileset");
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


        Water_Bar_Container.transform.position = new Vector3 (GetComponent<Transform>().position.x, GetComponent<Transform>().position.y + 0.4f, Water_Bar_Container.transform.position.z);

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            MapManager.dangerPopupsHolder.SetActive(true);
            gameManagerMap.player.SetActive(true);
            gameManagerMap.playingMiniGame = false;
            gameManagerMap.completedMiniGame = true; //asta trebuie pusa doar la castig
            SceneManager.LoadScene(0);
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
        else if(collider.tag == "Fire")
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                if (Water > 0)
                {
                    collider.gameObject.GetComponent<SpriteRenderer>().sprite = Sprites[455];
                    Water--;
                }
            }
        }

        WaterSlider.value = (float)Water / 4; 
        WaterLoadSlider.value = (float)seconds / 40;


        
    }
}
