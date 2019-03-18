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

    // Start is called before the first frame update
    void Start()
    {
        selectRequirements(); //start with the requirements tab
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.currentAction == 0 && reset != 0) { resetButtons(); selectRequirements(); }
    }

    private void resetButtons()
    {
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
}
