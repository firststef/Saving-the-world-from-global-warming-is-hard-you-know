using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ShowDetails : MonoBehaviour
{
    private TextMeshProUGUI detailsTitle;
    private TextMeshProUGUI details;
    private GameObject gameManagerObject;
    private GameManager gameManager;
    private MouseController mouseController;
    private CustomTile shownTile;
    private CustomTile currentTile;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerObject = GameObject.Find("GameManager");
        gameManager = gameManagerObject.GetComponent<GameManager>();
        mouseController = gameManagerObject.GetComponent<MouseController>();

        detailsTitle = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        details = transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.SelectionIsEnabled)
        {
            currentTile = GetShowTile();
            if (currentTile != shownTile)
            {
                if (currentTile != null) UpdateDetailsData();
                else EraseDetailsData();
                shownTile = currentTile;
            }
        }
        else EraseDetailsData();
    }

    private CustomTile GetShowTile ()
    {
        if (mouseController.checkIfOverUI())
        {
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            GameObject.Find("Canvas").GetComponent<GraphicRaycaster>().Raycast(ped, results);
            ObjectButtonScript script =null;
            foreach (RaycastResult result in results)
            {
                script = result.gameObject.GetComponent<ObjectButtonScript>();
                if (script != null) break;
            }
            if (script != null) return script.holdingTile; 
        }
        if (gameManager.selectedConstruction != null)
        {
            return gameManager.selectedConstruction;
        }
        else return gameManager.ReturnTileAbove(mouseController.currentCell, 0);
    }

    private void UpdateDetailsData()
    {
        details.text = currentTile.name;
    }

    private void EraseDetailsData()
    {
        details.text = "";
        detailsTitle.text = "";
    }
}
