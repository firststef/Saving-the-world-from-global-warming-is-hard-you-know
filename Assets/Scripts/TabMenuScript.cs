using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TabMenuScript : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject buildButton;
    public GameObject destroyButton;
    public GameObject upgradeButton;
    public GameObject eventsButton;
    public GameObject buildTab;
    public GameObject destroyTab;
    public GameObject upgradeTab;
    public GameObject eventsTab;
    public GameObject eventsList;
    public GameObject eventOptions;

    private int reset = 0;

    // Start is called before the first frame update
    void Start()
    {
        selectBuild(); //start with the build tab
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.currentAction == 0 && reset!=0) resetButtons();
    }

    private void resetButtons()
    {
        reset = 0;
#pragma warning disable CS0618 // Type or member is obsolete
        SetTransparent(buildTab.transform, 0);
        SetTransparent(destroyTab.transform, 0);
        SetTransparent(upgradeTab.transform, 0);
        SetTransparent(eventsTab.transform, 0);
#pragma warning restore CS0618 // Type or member is obsolete
        buildButton.GetComponent<Button>().interactable = true;
        destroyButton.GetComponent<Button>().interactable = true;
        upgradeButton.GetComponent<Button>().interactable = true;
        eventsButton.GetComponent<Button>().interactable = true;
    }

    public void selectBuild() //sa mut asta in scriptul meniului
    {
        reset = 1;
        gameManager.currentAction = 1;
#pragma warning disable CS0618 // Type or member is obsolete
        SetTransparent(buildTab.transform, 1);
        SetTransparent(destroyTab.transform, 0);
        SetTransparent(upgradeTab.transform, 0);
        SetTransparent(eventsTab.transform, 0);
#pragma warning restore CS0618 // Type or member is obsolete
        buildButton.GetComponent<Button>().interactable = false;
        destroyButton.GetComponent<Button>().interactable = true;
        upgradeButton.GetComponent<Button>().interactable = true;
        eventsButton.GetComponent<Button>().interactable = true;
    }

    public void selectDestroy()
    {
        reset = 1;
        gameManager.currentAction = 2;
#pragma warning disable CS0618 // Type or member is obsolete
        SetTransparent(buildTab.transform, 0);
        SetTransparent(destroyTab.transform, 1);
        SetTransparent(upgradeTab.transform, 0);
        SetTransparent(eventsTab.transform, 0);
#pragma warning restore CS0618 // Type or member is obsolete
        buildButton.GetComponent<Button>().interactable = true;
        destroyButton.GetComponent<Button>().interactable = false;
        upgradeButton.GetComponent<Button>().interactable = true;
        eventsButton.GetComponent<Button>().interactable = true;
    }

    public void selectUpgrade()
    {
        reset = 1;
        gameManager.currentAction = 3;
#pragma warning disable CS0618 // Type or member is obsolete
        SetTransparent(buildTab.transform, 0);
        SetTransparent(destroyTab.transform, 0);
        SetTransparent(upgradeTab.transform, 1);
        SetTransparent(eventsTab.transform, 0);
#pragma warning restore CS0618 // Type or member is obsolete
        buildButton.GetComponent<Button>().interactable = true;
        destroyButton.GetComponent<Button>().interactable = true;
        upgradeButton.GetComponent<Button>().interactable = false;
        eventsButton.GetComponent<Button>().interactable = true;
    }

    public void selectEvents()
    {
        reset = 1;
        gameManager.currentAction = 4;
#pragma warning disable CS0618 // Type or member is obsolete
        SetTransparent(buildTab.transform, 0);
        SetTransparent(destroyTab.transform, 0);
        SetTransparent(upgradeTab.transform, 0);
        SetTransparent(eventsTab.transform, 1);
        SetTransparent(eventsList.transform, 1);
        SetTransparent(eventOptions.transform,0);
#pragma warning restore CS0618 // Type or member is obsolete
        buildButton.GetComponent<Button>().interactable = true;
        destroyButton.GetComponent<Button>().interactable = true;
        upgradeButton.GetComponent<Button>().interactable = true;
        eventsButton.GetComponent<Button>().interactable = false;
    }

    public void showEventOptions(CustomTile holdingTile)
    {
        reset = 1;
        SetTransparent(eventsList.transform, 0);

        eventOptions.GetComponentInChildren<TextMeshProUGUI>().text = holdingTile.name; // de pus descriere

        foreach (Transform childT in eventOptions.transform)
        {
            if (childT.gameObject.name == "ButtonsHolder")
            {
                int numOfButtons = 3;
                int i = 0;
                foreach (Transform button in childT.transform)
                {
                    if (i < numOfButtons)
                    {
                        childT.GetChild(i).gameObject.SetActive(true);
                        childT.GetChild(i).GetComponentInChildren<Text>().text = "Button " + i; // de legat cu custom tile
                        childT.GetChild(i).GetComponentInChildren<ObjectButtonScript>().holdingTile = holdingTile;
                    }
                    else
                    {
                        childT.GetChild(i).gameObject.SetActive(false);
                    }
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
}
