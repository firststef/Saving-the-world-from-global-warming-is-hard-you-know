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
    public GameObject gameProgress;

    public float dangerPoints;
    public float maximumDanger = 300;
    public int EventTimer =0;

    private int totalGames = 10;
    public int wonGames = 0;
    public static bool result = false;
    public GameObject resultsWindow;

    ////////// Map objects
    public GameObject canvas;
    public GameObject player;
    public GameObject dangerSprite;
    public static GameObject dangerPopupsHolder;
    public Sprite plane;
    private int activeEvents = 0;
    public string HighlightName = null;
    private bool IsClicked = false;
    public Text progressText;

    ////////// Between scenes
    public bool playingMiniGame = false;
    public bool completedMiniGame = false;
    private int LastGame = -1;
    private int CurrentGame;

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
        ///Pre-start
        progressText.text = wonGames + " / " + totalGames;

        ///On Restart
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
            if (count == 1)
            {
                dangerBar = canvasgo.transform.GetChild(0).GetComponent<Image>(); DontDestroyOnLoad(canvasgo);
                //GameObject gameProgress = canvasgo.transform.GetChild(1);
            }
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

        if (dangerPoints > maximumDanger) { result = false; ResultWindow(result); }

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
                    gameProgress.SetActive(false);
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

            wonGames++;
            progressText.text = wonGames + " / " + totalGames;

            if (wonGames == totalGames)
            {
                playingMiniGame = true; //folosit doar ca sa opreasca iteratia
                GameObject canvas = dangerBar.transform.parent.gameObject;
                foreach(Transform child in canvas.transform)
                {
                    child.gameObject.SetActive(false);
                }
                result = true;
                ResultWindow(result);
            }

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
        
        Vector3 dir = eventList[(int)location].position - player.transform.position;
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        player.transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);

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
        gameProgress.SetActive(false);

        SceneManager.LoadScene(GetGameFromLocation());
        IsClicked = false;
    }

    int GetGameFromLocation()
    {
        do
        {
            CurrentGame = (int)UnityEngine.Random.Range(2, 6);
        } while (CurrentGame == LastGame);

        LastGame = CurrentGame;

        return CurrentGame;
    }

    void CreateRandomEvent()
    {
            int index;
            do
            {
                index = UnityEngine.Random.Range(0, 7);
            }
            while (eventList[index].isActive == true); //folosit ca sa nu fie jucat de 2 ori acelasi nivel consecutiv
            
            eventList[index].isActive = true;
            Vector3 pos = eventList[index].position;
            pos.z = 0;
            GameObject instance = Instantiate(dangerSprite, pos, new Quaternion(0, 0, 0, 0),dangerPopupsHolder.transform);
    }

    public void ResultWindow(bool result)
    {
        GameObject a = GameObject.Find("ResultWindow");
        if ( a != null) return;
        GameObject resWindow = Instantiate(resultsWindow, canvas.transform);
        resWindow.name = "ResultWindow";
        resWindow.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().fillAmount = (float)wonGames / totalGames;
        if (result)
        {
            resWindow.transform.GetChild(1).GetComponent<Text>().text = "You Won!";
            resWindow.transform.GetChild(2).gameObject.SetActive(true);
            resWindow.transform.GetChild(4).transform.GetChild(0).GetComponent<Text>().text = "Main Menu";
        }
        else
        {
            resWindow.transform.GetChild(1).GetComponent<Text>().text = "Yet you were so close!";
            resWindow.transform.GetChild(3).gameObject.SetActive(true);
            resWindow.transform.GetChild(4).transform.GetChild(0).GetComponent<Text>().text = "Retry";
        }
    }

    public void WindowButton()
    {
        if (result)
        {
            var objects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject o in objects)
            {
                Destroy(o.gameObject);
            }
            SceneManager.LoadScene(0);//Load main menu
        }
        else
        {
            var objects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject o in objects)
            {
                Destroy(o.gameObject);
            }
            SceneManager.LoadScene(1);//Reload same scene
        }

    }
}
