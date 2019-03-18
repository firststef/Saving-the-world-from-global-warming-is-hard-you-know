using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ObjectButtonScript : MonoBehaviour
{
    public CustomTile holdingTile;
    private GameObject tabMenu;
    private TabMenuScript tabMenuScript;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        tabMenu = GameObject.Find("TabMenu");
        tabMenuScript = tabMenu.GetComponent<TabMenuScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick()
    {
        if (gameManager.currentAction == 4) gameManager.selectUpgrade(holdingTile);

        gameManager.selectedConstruction = holdingTile;


    }

    public void OnEventClick()
    {
        gameManager.selectedConstruction = null; //poate functiona si cu evenimente goale

        tabMenuScript.showEventOptions(holdingTile);
    }

    public void OnEventOptionButtonClick(int buttonIndex)
    {
        gameManager.ResolveEventOption(holdingTile, buttonIndex);
    }
}