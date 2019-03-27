using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerController2 : MonoBehaviour
{
    private MapManager gameManagerMap;
    public Tilemap tilemap;
    private Vector3 cameraLimitBottomLeftCorner;
    private Vector3 cameraLimitTopRightCorner;
    Vector3 limitBottomLeft;
    Vector3 limitTopRight;

    private Rigidbody2D rb;
    private Animator animator;
    private bool IsMoving;
    private Vector2 lastMove;
    public float MoveSpeed = 0.2f;

    public int BenguinNumber;
    public int[] ChosenPos;
    public Transform[] BenguinPositions;
    public GameObject Benguin;
    public GameObject Benguins;
    public GameObject Arrow;
    public GameObject exit;

    // Start is called before the first frame update
    void Start()
    {
        //Pre-level 
        gameManagerMap = GameObject.Find("GameManagerMap").GetComponent<MapManager>();
        gameManagerMap.player.SetActive(false);

        //BoundsCalculations
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        cameraLimitBottomLeftCorner = tilemap.CellToWorld(new Vector3Int(bounds.xMin, bounds.yMin, bounds.zMin));
        cameraLimitTopRightCorner = tilemap.CellToWorld(new Vector3Int(bounds.xMax, bounds.yMax, bounds.zMax - 1));
        limitBottomLeft = cameraLimitBottomLeftCorner;
        limitTopRight = cameraLimitTopRightCorner;
        var top = Camera.main.orthographicSize;
        var left = top * Camera.main.aspect;
        limitBottomLeft.x += left;
        limitBottomLeft.y += top;
        limitTopRight.x -= left;
        limitTopRight.y -= top;

        //Level
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        BenguinNumber = (int)Random.Range(1, 4);
        ChosenPos = new int[7];
        for (int i = 0; i < BenguinNumber; i++)
        {
            while (true)
            {
                int pos = Random.Range(1, 7);
                if (ChosenPos[pos] == 0)
                {
                    GameObject benguin = Instantiate(Benguin, BenguinPositions[pos].position, new Quaternion(0, 0, 0, 0), Benguins.transform);
                    ChosenPos[pos] = 1;
                    GameObject arrow = Instantiate(Arrow, gameObject.transform);
                    arrow.GetComponent<TargetIndicator>().Target = benguin.transform;
                    break;
                }
            }
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

    private void LateUpdate()
    {
        Vector3 feetPosition = transform.position;
        feetPosition.y -= (float)0.48;
        Vector3Int currentCell = tilemap.WorldToCell(feetPosition);
        currentCell.z = 0;
        if (tilemap.GetTile(currentCell) == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        bool isNearExit = (Mathf.Abs(exit.transform.position.x - transform.position.x) <= 0.5 && Mathf.Abs(exit.transform.position.y - transform.position.y) <= 0.5);
        if (BenguinNumber == 0 && isNearExit)
        {
            gameManagerMap.completedMiniGame = true;
            gameManagerMap.playingMiniGame = false;
            SceneManager.LoadScene(1);
            gameManagerMap.player.SetActive(true);
            gameManagerMap.gameProgress.SetActive(true);
            MapManager.dangerPopupsHolder.SetActive(true);
        }

        CameraLocation();
    }

    private void CameraLocation()
    {
        Vector3 pos = transform.position;
        pos.z = Camera.main.transform.position.z;
        if (!(limitBottomLeft.x <= pos.x && pos.x <= limitTopRight.x) && !(limitBottomLeft.x >= pos.x && pos.x >= limitTopRight.x))
        {
            pos.x = Camera.main.transform.position.x;
        }
        if (!(limitBottomLeft.y <= pos.y && pos.y <= limitTopRight.y) && !(limitBottomLeft.y >= pos.y && pos.y >= limitTopRight.y))
        {
            pos.y = Camera.main.transform.position.y;
        }
        Camera.main.transform.Translate(pos - Camera.main.transform.position);
    }

}
