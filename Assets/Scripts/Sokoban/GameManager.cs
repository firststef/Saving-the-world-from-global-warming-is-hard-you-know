using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    MapManager gameManagerMap;

    public LevelBuilder m_LevelBuilder;
    private bool m_ReadyForInput;
    private Player m_Player;
    private Level currentLevel;
    private int LevelNumber;

    public int NumberOfCrosses;

    void Start()
    {
        gameManagerMap = GameObject.Find("GameManagerMap").GetComponent<MapManager>();
        gameManagerMap.player.SetActive(false);

        LevelNumber = GameObject.Find("Levels").GetComponent<Levels>().m_Levels.Count;
        currentLevel = GameObject.Find("Levels").GetComponent<Levels>().m_Levels[(int)Random.Range(0, LevelNumber)];

        m_LevelBuilder.Build(currentLevel);
        m_Player = FindObjectOfType<Player>();
    }

    void Update()
    {
        if (NumberOfCrosses == 0 || Input.GetKeyDown(KeyCode.Escape))
        {
            if(Input.GetKeyDown(KeyCode.Escape))
                gameManagerMap.completedMiniGame = false;
            else
                gameManagerMap.completedMiniGame = true;

            gameManagerMap.playingMiniGame = false;
            SceneManager.LoadScene(1);
            gameManagerMap.player.SetActive(true);
            MapManager.dangerPopupsHolder.SetActive(true);
            gameManagerMap.gameProgress.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            m_LevelBuilder.Build(currentLevel);
            m_Player = FindObjectOfType<Player>();
        }

        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveInput.Normalize();
        if (moveInput.sqrMagnitude > 0.5)
        {
            if (m_ReadyForInput)
            {
                m_ReadyForInput = false;
                m_Player.Move(moveInput);
            }
        }
        else m_ReadyForInput = true;
    }
}
