using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ObjectHolderScript : MonoBehaviour
{
    public GameObject objectButton;
    public GameObject objectEventButton;
    public GameObject requirementsItem;
    public string List;
    private GameManager gameManager;
    private List<GameManager.DatabaseItem> tileList;
    private List<GameManager.DatabaseItemBase> tileDatabase;
    private List<GameManager.DatabaseItemEvent> eventDatabase;
    private List<GameManager.DatabaseItemRequirement> requirementsDatabase;
    private GridLayout gridLayout;
    public bool listUpToDate = false;
    private int currentFrame = 0;
    public CustomTile currentTileUpgrade; //make private

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        tileDatabase = gameManager.tileDatabase;
        eventDatabase = gameManager.eventDatabase;
        requirementsDatabase = gameManager.requirementsDatabase;
        gridLayout = GetComponent<GridLayout>();

        if (List == "Requirements") UpdateRequirements();
    }

    // Update is called once per frame
    void Update()
    {
        if (List == "Constructions" && !listUpToDate)
        {
            foreach (GameManager.DatabaseItemBase databaseItem in tileDatabase)
            {
                if (databaseItem.unlocked)
                {
                    CustomTile tile = databaseItem.tile;
                    GameObject instance = Instantiate(objectButton, transform);
                    instance.name = "ObjectButton=>" + tile.name;
                    instance.GetComponent<ObjectButtonScript>().holdingTile = tile;
                    if (databaseItem.picture != null)
                        instance.transform.GetChild(0).GetComponent<Image>().sprite = databaseItem.picture;
                    else
                        instance.transform.GetChild(0).GetComponent<Image>().sprite = tile.sprite;
                }
            }
            listUpToDate = true;
            return;
        }
        else if (List == "Upgrades")
        {
            if (gameManager.selectedConstruction == null || currentTileUpgrade != gameManager.selectedConstruction)
            {
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
                currentTileUpgrade = null;
            }
            if (gameManager.selectedConstruction != null && currentTileUpgrade == null)
            {
                currentTileUpgrade = gameManager.selectedConstruction;
                foreach (GameManager.DatabaseItemBase databaseItem in tileDatabase)
                {
                    CustomTile tile = databaseItem.tile;
                    bool fromTheStart = false;
                    if (currentTileUpgrade == tile)
                        fromTheStart = true;
                    tileList = databaseItem.upgrades;
                    bool foundNextUpgrade = false;
                    foreach (GameManager.DatabaseItem tileu in tileList)
                    {
                        if ((fromTheStart || foundNextUpgrade) && tileu != null)
                        {
                            if (tileu.unlocked)
                            {
                                GameObject instance = Instantiate(objectButton, transform);
                                instance.name = "ObjectButton=>" + tileu.tile.name;
                                instance.GetComponent<ObjectButtonScript>().holdingTile = tileu.tile;
                                if (tileu.picture != null)
                                    instance.transform.GetChild(0).GetComponent<Image>().sprite = tileu.picture;
                                else
                                    instance.transform.GetChild(0).GetComponent<Image>().sprite = tileu.tile.sprite;
                            }
                            if (tileu.tile == currentTileUpgrade && !foundNextUpgrade) foundNextUpgrade = true;
                        }
                    }
                }
            }
            return;
        }
        else if (List == "Events" && !listUpToDate)
        {
            int id = 0;
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            foreach (GameManager.DatabaseItemEvent databaseItemEvent in eventDatabase)
            {
                CustomTile tile = databaseItemEvent.tile;
                GameObject instance = Instantiate(objectEventButton, transform);
                instance.name = id++ + " ObjectButton=>" + tile.name;
                instance.GetComponent<ObjectButtonScript>().holdingTile = tile;
                instance.transform.GetChild(0).GetComponent<Text>().text = tile.name;
                instance.transform.GetChild(1).GetComponent<Text>().text = tile.description;
                listUpToDate = true;
            }
            return;
        }
        else if (List == "Requirements")
        {
            currentFrame++;
            if (currentFrame >= 60)
            {
                UpdateRequirements();
                currentFrame = 0;
            }
        }        
    }

    private void UpdateRequirements()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        foreach (GameManager.DatabaseItemRequirement databaseItemRequirement in requirementsDatabase)
        {
            CustomTile tile = databaseItemRequirement.tile;
            GameObject instance = Instantiate(requirementsItem, transform);
            instance.name = "ObjectItem=>" + databaseItemRequirement.title;
            instance.transform.GetChild(0).GetComponent<Text>().text = databaseItemRequirement.title;
            if (tile != null)
            {
                instance.transform.GetChild(2).GetComponent<Text>().text = "Currently you have: " + gameManager.retVariableForRequirement(tile) + "/" + databaseItemRequirement.amount;
                if (gameManager.retVariableForRequirement(tile) >= databaseItemRequirement.amount)
                {
                    instance.transform.GetChild(3).gameObject.SetActive(true);
                    databaseItemRequirement.completed = true;
                }
                else
                {
                    instance.transform.GetChild(3).gameObject.SetActive(false);
                    databaseItemRequirement.completed = false;
                }
            }
            else
            {
                instance.transform.GetChild(2).GetComponent<Text>().text = "Currently you have: " + gameManager.retVariableForRequirement(databaseItemRequirement.variable) + "/" + databaseItemRequirement.amount;
                if (gameManager.retVariableForRequirement(databaseItemRequirement.variable) >= databaseItemRequirement.amount)
                {
                    instance.transform.GetChild(3).gameObject.SetActive(true);
                    databaseItemRequirement.completed = true;
                }
                else
                {
                    instance.transform.GetChild(3).gameObject.SetActive(false);
                    databaseItemRequirement.completed = false;
                }
            }

            instance.transform.GetChild(1).GetComponent<Text>().text = databaseItemRequirement.description;
        }
        return;
    }
}
