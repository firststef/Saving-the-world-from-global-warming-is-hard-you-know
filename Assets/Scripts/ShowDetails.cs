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
        //as putea face if current action 5 si este peste un buton de event care are un tile afiseaza optiunea
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
                if (script != null && gameManager.currentAction != 5) break;
            }
            if (script != null) return script.holdingTile;
            if (gameManager.selectedConstruction != null)
            {
                return gameManager.selectedConstruction;
            }
            return null;
        }
        if (gameManager.selectedConstruction != null)
        {
            return gameManager.selectedConstruction;
        }
        return gameManager.ReturnTileAbove(mouseController.currentCell, 0);
    }

    private void UpdateDetailsData()
    {
        detailsTitle.text = (currentTile.displayName == "") ? currentTile.name :currentTile.displayName;
        details.text = currentTile.description;
    }

    private void EraseDetailsData()
    {
        details.text = "";
        detailsTitle.text = "";
    }
}
