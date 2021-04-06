using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

using TMPro;

public class TabMenuScript : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject requirementsButton;
    public GameObject buildButton;
    public GameObject destroyButton;
    public GameObject upgradeButton;
    public GameObject eventsButton;
    public GameObject requirementsTab;
    public GameObject buildTab;
    public GameObject destroyTab;
    public GameObject upgradeTab;
    public GameObject eventsTab;
    public GameObject eventsList;
    public GameObject eventOptions;

    private int reset = 0;

    int buildZoneHeight = 4;

    // Start is called before the first frame update
    void Start()
    {
        selectRequirements(); //start with the requirements tab
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.currentAction == 0 && reset != 0) { resetButtons(); selectRequirements(); gameManager.currentAction = 0; }
    }

    private void resetButtons()
    {
        EraseBuildZone();
        reset = 0;
#pragma warning disable CS0618 // Type or member is obsolete
        SetTransparent(requirementsTab.transform, 0);
        SetTransparent(buildTab.transform, 0);
        SetTransparent(destroyTab.transform, 0);
        SetTransparent(upgradeTab.transform, 0);
        SetTransparent(eventsTab.transform, 0);
#pragma warning restore CS0618 // Type or member is obsolete
        requirementsButton.GetComponent<Button>().interactable = true;
        buildButton.GetComponent<Button>().interactable = true;
        destroyButton.GetComponent<Button>().interactable = true;
        upgradeButton.GetComponent<Button>().interactable = true;
        eventsButton.GetComponent<Button>().interactable = true; 
    }

    public void selectRequirements() //sa mut asta in scriptul meniului
    {
        resetButtons();
        reset = 1;
        gameManager.currentAction = 1;
#pragma warning disable CS0618 // Type or member is obsolete
        SetTransparent(requirementsTab.transform, 1);
#pragma warning restore CS0618 // Type or member is obsolete
        requirementsButton.GetComponent<Button>().interactable = false;
    }

    public void selectBuild() //sa mut asta in scriptul meniului
    {
        resetButtons();
        reset = 1;
        gameManager.currentAction = 2;
#pragma warning disable CS0618 // Type or member is obsolete
        SetTransparent(buildTab.transform, 1);
#pragma warning restore CS0618 // Type or member is obsolete
        buildButton.GetComponent<Button>().interactable = false;
        ShowBuildZone();
    }

    public void selectDestroy()
    {
        resetButtons();
        reset = 1;
        gameManager.currentAction = 3;
#pragma warning disable CS0618 // Type or member is obsolete
        SetTransparent(destroyTab.transform, 1);
#pragma warning restore CS0618 // Type or member is obsolete
        destroyButton.GetComponent<Button>().interactable = false;
    }

    public void selectUpgrade()
    {
        resetButtons();
        reset = 1;
        gameManager.currentAction = 4;
#pragma warning disable CS0618 // Type or member is obsolete
        SetTransparent(upgradeTab.transform, 1);
#pragma warning restore CS0618 // Type or member is obsolete
        upgradeButton.GetComponent<Button>().interactable = false;
    }

    public void selectEvents()
    {
        resetButtons();
        reset = 1;
        gameManager.currentAction = 5;
#pragma warning disable CS0618 // Type or member is obsolete
        SetTransparent(eventsTab.transform, 1);
        SetTransparent(eventsList.transform, 1);
        SetTransparent(eventOptions.transform,0);
#pragma warning restore CS0618 // Type or member is obsolete
        eventsButton.GetComponent<Button>().interactable = false;
    }

    public void showEventOptions(CustomTile holdingTile)
    {
        reset = 1;
        SetTransparent(eventsList.transform, 0);

        eventOptions.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = holdingTile.name;
        eventOptions.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = holdingTile.description;

        foreach (Transform childT in eventOptions.transform)
        {
            if (childT.gameObject.name == "ButtonsHolder")
            {
                int i = 0;
                foreach (Transform button in childT.transform)
                {
                    childT.GetChild(i).gameObject.SetActive(true);
                    childT.GetChild(i).GetComponentInChildren<ObjectButtonScript>().holdingTile = holdingTile;
                    if (((i == 0) ? holdingTile.button1 : ((i == 1) ? holdingTile.button2 : holdingTile.button3)) != "")
                        childT.GetChild(i).GetComponentInChildren<Text>().text = (i == 0) ? holdingTile.button1 : ((i == 1) ? holdingTile.button2 : holdingTile.button3);
                    else
                        childT.GetChild(i).gameObject.SetActive(false);
                    i++;
                }
            } 
        }
        SetTransparent(eventOptions.transform, 1);
    }

    public void SetTransparent(Transform transform, float transparency)
    {
        transform.gameObject.GetComponent<CanvasGroup>().alpha = transparency;
        if (transparency == 0)
        {
            transform.gameObject.GetComponent<CanvasGroup>().interactable = false;
            transform.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            transform.gameObject.GetComponent<CanvasGroup>().interactable = true;
            transform.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    public void ShowBuildZone()
    {
        foreach (GameManager.DatabaseItemBase baseItem in gameManager.tileDatabase)
        {
            if ((baseItem.tile.displayName != "City")&& (baseItem.tile.displayName != "Pole")) continue;

            foreach (Vector3Int location in baseItem.locations)
            {
                ShowSurroundingBuildZone(location);
            }

            foreach(GameManager.DatabaseItem item in baseItem.upgrades)
            {
                if ((baseItem.tile.displayName != "City")&& (baseItem.tile.displayName != "Pole")) continue;

                foreach (Vector3Int location in item.locations)
                {
                    ShowSurroundingBuildZone(location);
                }
            }
        }
    }

    public void ShowSurroundingBuildZone(Vector3Int cell)
    {
        if (cell.y % 2 == 0)
        {
            gameManager.tilemap.SetTile(new Vector3Int(cell.x , cell.y, buildZoneHeight), gameManager.selectionTile);;
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y + 1, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x, cell.y + 1, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x, cell.y - 1, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y - 1, buildZoneHeight), gameManager.buildZoneTile);

            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y + 2, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 0, cell.y + 2, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y + 2, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y + 1, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 2, cell.y, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y - 1, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y - 2, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x, cell.y - 2, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y - 2, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 2, cell.y - 1, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 2, cell.y + 0, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 2, cell.y + 1, buildZoneHeight), gameManager.buildZoneTile);
        }
        else
        {
            gameManager.tilemap.SetTile(new Vector3Int(cell.x, cell.y, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y + 2, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 0, cell.y + 2, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y + 2, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y + 1, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x , cell.y + 1, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y + 1, buildZoneHeight), gameManager.buildZoneTile);

            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 2, cell.y + 1, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 2, cell.y + 0, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y , buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y , buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 2, cell.y, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y - 1, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x , cell.y - 1, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y - 1, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 2, cell.y - 1, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y - 2, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x , cell.y -2, buildZoneHeight), gameManager.buildZoneTile);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x +1, cell.y -2, buildZoneHeight), gameManager.buildZoneTile);
        }

        return;

    }

    public void EraseSurroundingBuildZone(Vector3Int cell)
    {
        if (cell.y % 2 == 0)
        {
            gameManager.tilemap.SetTile(new Vector3Int(cell.x, cell.y, buildZoneHeight),null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y + 1, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x, cell.y + 1, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x, cell.y - 1, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y - 1, buildZoneHeight), null);

            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y + 2, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 0, cell.y + 2, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y + 2, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y + 1, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 2, cell.y, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y - 1, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y - 2, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x, cell.y - 2, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y - 2, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 2, cell.y - 1, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 2, cell.y + 0, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 2, cell.y + 1, buildZoneHeight), null);
        }
        else
        {
            gameManager.tilemap.SetTile(new Vector3Int(cell.x, cell.y, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y + 2, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 0, cell.y + 2, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y + 2, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y + 1, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x, cell.y + 1, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y + 1, buildZoneHeight), null);

            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 2, cell.y + 1, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 2, cell.y + 0, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 2, cell.y, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y - 1, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x, cell.y - 1, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y - 1, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 2, cell.y - 1, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x - 1, cell.y - 2, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x, cell.y - 2, buildZoneHeight), null);
            gameManager.tilemap.SetTile(new Vector3Int(cell.x + 1, cell.y - 2, buildZoneHeight), null);
        }

        return;
    }

    public void EraseBuildZone()
    {
        foreach (GameManager.DatabaseItemBase baseItem in gameManager.tileDatabase)
        {
            if ((baseItem.tile.displayName != "City") && (baseItem.tile.displayName != "Pole")) continue;

            foreach (Vector3Int location in baseItem.locations)
            {
                EraseSurroundingBuildZone(location);
            }

            foreach (GameManager.DatabaseItem item in baseItem.upgrades)
            {
                if ((baseItem.tile.displayName != "City") && (baseItem.tile.displayName != "Pole")) continue;

                foreach (Vector3Int location in item.locations)
                {
                    EraseSurroundingBuildZone(location);
                }
            }
        }
    }
}
