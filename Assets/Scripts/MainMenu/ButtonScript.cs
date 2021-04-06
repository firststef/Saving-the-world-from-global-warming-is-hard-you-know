using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject optionsCanvas;

    public Button StartGameButton, OptionsButton, QuitButton, CampaignButton;

    void Start()
    {
        StartGameButton.onClick.AddListener(StartGame);
        OptionsButton.onClick.AddListener(Options);
        QuitButton.onClick.AddListener(QuitGame);
        CampaignButton.onClick.AddListener(Campaign);
    }

    void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    void Options()
    {
        mainCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    void QuitGame()
    {
        Application.Quit();
    }

    void Campaign()
    {
        SceneManager.LoadScene(6);
    }
}
