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
    public Sprite plane;
    private int activeEvents = 0;
    public string HighlightName = null;
    private bool IsClicked = false;

    ////////// Between scenes
    public bool playingMiniGame = false;
    public bool completedMiniGame = false;
    private int LastGame = -1;

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
        {
            EventTimer++;
            if (activeEvents == 0)
                dangerPoints -= Time.deltaTime * 6;
            if (dangerPoints < 0) dangerPoints = 0;
        }
        else
        {
            EventTimer = 0;

            if (activeEvents < eventList.Count)
            {
                CreateRandomEvent(); activeEvents++;
            }
        }

        /////////// On-click - Move to mini-game
        if (Input.GetMouseButton(0) && HighlightName != null && !IsClicked)
        {
            IsClicked = true;

            if ((int)SetDestination(HighlightName) < eventList.Count && eventList[(int)SetDestination(HighlightName)].isActive)
            {
                playingMiniGame = true;
                if (location != SetDestination(HighlightName))
                {
                    location = SetDestination(HighlightName);
                    playingMiniGame = true;
                    StartCoroutine(FlyingAnimation());
                }
                else
                {
                    location = SetDestination(HighlightName);
                    player.transform.position = eventList[(int)location].position;
                    dangerPopupsHolder.SetActive(false);
                    SceneManager.LoadScene(GetGameFromLocation());
                    IsClicked = false;
                }
                return;
            }
        }
        if (Input.GetMouseButtonUp(0))
            IsClicked = false;

        /////////// If mini-game was completed, erase Dangerbutton
        if (completedMiniGame)
        {
            Vector3 pos = eventList[(int)location].position;
            pos.z = 0;
            foreach (Transform child in dangerPopupsHolder.transform)
            {
                if (child.transform.position == pos) { Destroy(child.gameObject); activeEvents--; }
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

    private IEnumerator FlyingAnimation()
    {
        Sprite spr = player.GetComponent<SpriteRenderer>().sprite;
        player.GetComponent<SpriteRenderer>().sprite = plane;

        Vector3 dir = eventList[(int)location].position - transform.position;
        float angle = 90-(1/Mathf.Atan2(dir.y, dir.x)) * Mathf.Rad2Deg;
        player.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 start = player.transform.position;
        Vector3 dest = eventList[(int)location].position;
        Vector3 pos;

        for (int i = 0; i <= 100; i++)
        {
            pos.x = (dest.x * i + start.x * (100 - i)) / 100;
            pos.y = (dest.y * i + start.y * (100 - i)) / 100;
            pos.z = 0;

            player.transform.position = pos;
            yield return new WaitForSeconds(0.01f);
        }

        player.GetComponent<SpriteRenderer>().sprite = spr;
        player.transform.rotation = new Quaternion(0,0,0,0);

        dangerPopupsHolder.SetActive(false);

        SceneManager.LoadScene(GetGameFromLocation());
        IsClicked = false;
    }

    int GetGameFromLocation()
    {
        if (location == Continents.America) return 1;
        if (location == Continents.Greenland) return 2;
        return 1;
    }

    /*
    private void MovePlayer()
    {
        Vector3 pos = eventList[(int)location].position;
        pos.z = 0;
        pos.y -= 0.06f;
        player.transform.position = pos;
    }
    */
    void CreateRandomEvent()
    {
        while (true)
        {
            int index = LastGame;
            while (index == LastGame) //folosit ca sa nu fie jucat de 2 ori acelasi nivel consecutiv
            {
                index = UnityEngine.Random.Range(0, 7);
            }
            LastGame = index;
            if (eventList[index].isActive == false)
            {
                eventList[index].isActive = true;
                Vector3 pos = eventList[index].position;
                pos.z = 0;
                GameObject instance = Instantiate(dangerSprite, pos, new Quaternion(0, 0, 0, 0),dangerPopupsHolder.transform);
                
                return;
            }
        }
    }
}
