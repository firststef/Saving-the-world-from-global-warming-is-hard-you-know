using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    Scene m_Scene;
    public AudioMixerSnapshot MainMenu;
    public AudioMixerSnapshot Game;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");

        if (objs.Length > 1)
            Destroy(objs[1]);

        DontDestroyOnLoad(objs[0]);
    }

    void Update()
    {
        m_Scene = SceneManager.GetActiveScene();
        if (m_Scene.name == "MainMenu" || m_Scene.name == "OptionsMenu")
            MainMenu.TransitionTo(0f);
        else Game.TransitionTo(0f);
    }
}
