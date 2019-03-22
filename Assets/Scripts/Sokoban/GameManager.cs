using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public LevelBuilder m_LevelBuilder;
    private bool m_ReadyForInput;
    private Player m_Player;

    public int NumberOfCrosses;

    void Start()
    {
        m_LevelBuilder.Build();
        m_Player = FindObjectOfType<Player>();
    }

    void Update()
    {
        if (NumberOfCrosses == 0)
        {
            SceneManager.LoadScene(0);
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
