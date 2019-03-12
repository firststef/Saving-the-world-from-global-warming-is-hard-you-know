using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ObjectHolderScript : MonoBehaviour
{
    public GameObject objectButton;
    public GameObject objectEventButton;
    public string List;
    private GameManager gameManager;
    private List<GameManager.DatabaseItem> tileList;
    private List<GameManager.DatabaseItemBase> tileDatabase;
    private List<GameManager.DatabaseItemEvent> eventDatabase;
    private GridLayout gridLayout;
    public bool listUpToDate = false;
    public CustomTile currentTileUpgrade; //make private

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        tileDatabase = gameManager.tileDatabase;
        eventDatabase = gameManager.eventDatabase;
        gridLayout = GetComponent<GridLayout>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (List == "Constructions" && !listUpToDate)
        {
            foreach (GameManager.DatabaseItemBase databaseItem in tileDatabase)
            {
                CustomTile tile = databaseItem.tile;
                GameObject instance = Instantiate(objectButton, transform);
                instance.name = "ObjectButton=>" + tile.name;
                instance.GetComponent<ObjectButtonScript>().holdingTile = tile;
                instance.GetComponent<Image>().sprite = tile.sprite;
                listUpToDate = true;
            }
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
                            GameObject instance = Instantiate(objectButton, transform);
                            instance.name = "ObjectButton=>" + tileu.tile.name;
                            instance.GetComponent<ObjectButtonScript>().holdingTile = tileu.tile;
                            instance.GetComponent<Image>().sprite = tileu.tile.sprite;
                        }
                        if (tileu.tile == currentTileUpgrade && !foundNextUpgrade) foundNextUpgrade = true;
                    }
                }
            }

            return;
        }
        else if (List == "Events" && !listUpToDate)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            foreach (GameManager.DatabaseItemEvent databaseItemEvent in eventDatabase)
            {
                CustomTile tile = databaseItemEvent.tile;
                GameObject instance = Instantiate(objectEventButton, transform);
                instance.name = "ObjectButton=>" + tile.name;
                instance.GetComponent<ObjectButtonScript>().holdingTile = tile;
                instance.transform.GetChild(0).GetComponent<Text>().text = tile.name;
                instance.transform.GetChild(1).GetComponent<Text>().text = "Descriere";
                listUpToDate = true;
            }
            return;
        }
    }
}
