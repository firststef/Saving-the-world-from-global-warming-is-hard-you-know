using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    ////////// This Object
    public static GameObject gameManagerMap;

    ////////// Game progress
    public static Image dangerBar;
    private Image dangerBarContent;

    public float dangerPoints;
    public float maximumDanger = 300;
    public int EventTimer =0;

    ////////// Map objects
    public GameObject player;
    public GameObject dangerSprite;
    public static GameObject dangerPopupsHolder;
    private int activeEvents = 0;
    public string HighlightName = null;
    private bool IsClicked = false;

    ////////// Between scenes
    public bool playingMiniGame = false;
    public bool completedMiniGame = false;

    [Serializable]
    public class Event
    {
        [SerializeField] public bool isActive;
        [SerializeField] public Vector3 position;
    }
    public List<Event> eventList;

    public enum Continents
    {
        Europe = 0,
        Russia,  
        China,
        Africa,
        America,
        Greenland,
        SouthAmerica,
        NULL
    };
    public Continents location = Continents.Europe;

    // Start is called before the first frame update
    void Start()
    { 
        if (gameManagerMap == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameManagerMap = this.gameObject;
        }
        else Destroy(this.gameObject);

        player = GameObject.Find("MapPlayer");
        int count = 0;
        foreach (GameObject po in GameObject.FindGameObjectsWithTag("Player"))
        {
            count++;
            if (count == 1) { DontDestroyOnLoad(po); }
            if (count >= 2) Destroy(po);
        }

        count = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("DangerPopupsHolder"))
        {
            count++;
            if (count == 1) { dangerPopupsHolder = go; DontDestroyOnLoad(go); }
            if (count >= 2) Destroy(go);
        }

        count = 0;
        foreach (GameObject canvasgo in GameObject.FindGameObjectsWithTag("Canvas"))
        {
            count++;
            if (count == 1) { dangerBar = canvasgo.transform.GetChild(0).GetComponent<Image>(); DontDestroyOnLoad(canvasgo); }
            if (count >= 2) Destroy(canvasgo);
        }

        dangerBarContent = dangerBar.transform.GetChild(0).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        /////////// Fill the bar
        dangerBarContent.fillAmount = dangerPoints / maximumDanger;

        dangerPoints += activeEvents * Time.deltaTime *3;

        /////////// If playing mini game - stop until this point
        if (playingMiniGame) return;

        /////////// Danger Pop-ups
        if (EventTimer < 300)
            EventTimer++;
        else
        {
            EventTimer = 0;
            if (activeEvents < eventList.Count) { CreateRandomEvent(); activeEvents++; }
        }

        /////////// On-click - Move to mini-game
        if (Input.GetMouseButton(0) && HighlightName != null && !IsClicked)
        {
            IsClicked = true;

            if ((int)SetDestination(HighlightName) < eventList.Count && eventList[(int)SetDestination(HighlightName)].isActive)
            {
                location = SetDestination(HighlightName);
                MovePlayer();
                dangerPopupsHolder.SetActive(false);
                playingMiniGame = true;
                SceneManager.LoadScene(1);
            }
        }
        if (Input.GetMouseButtonUp(0))
            IsClicked = false;

        /////////// If mini-game was completed, erase Dangerbutton
        if (completedMiniGame)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(eventList[(int)location].position);
            pos.z = 0;
            foreach (Transform child in dangerPopupsHolder.transform)
            {
                if (child.transform.position == pos) { Destroy(child.gameObject); }
            }
            eventList[(int)location].isActive = false;
            completedMiniGame = false;
        }
    }

    private Continents SetDestination(string HighlightName)
    {
        switch(HighlightName)
        {
            case "Europe":
                return Continents.Europe;
            case "Russia":
                return Continents.Russia;
            case "China":
                return Continents.China;
            case "Africa":
                return Continents.Africa;
            case "America":
                return Continents.America;
            case "Greenland":
                return Continents.Greenland;
            case "South America":
                return Continents.SouthAmerica;
            default:
                return Continents.NULL;
        }
    }

    private void MovePlayer()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(eventList[(int)location].position);
        pos.z = 0;
        pos.y -= 0.06f;
        player.transform.position = pos;
    }

    void CreateRandomEvent()
    {
        while (true)
        {
            int index = (int)UnityEngine.Random.Range(0, 7);
            if (eventList[index].isActive == false)
            {
                eventList[index].isActive = true;
                Vector3 pos = Camera.main.ScreenToWorldPoint(eventList[index].position);
                pos.z = 0;
                GameObject instance = Instantiate(dangerSprite, pos, new Quaternion(0, 0, 0, 0),dangerPopupsHolder.transform);
                
                return;
            }
        }
    }
}
