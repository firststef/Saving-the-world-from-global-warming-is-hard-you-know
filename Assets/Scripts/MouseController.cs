using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MouseController : MonoBehaviour
{
    private GameManager gameManager;
    private Tilemap tilemap;

    //////////////////////////// UI
    private GameObject canvas;
    private GameObject menuPanel;
    public GameObject windowPrefab = null;
    public List<GameObject> windowList;
    private bool panelUp = true;
    private float hideY;

    ///////////////// Event UI
    public GameObject eventPopupPrefab = null;
    private bool cellEventPopupActive = false;
    private GameObject eventPopup;
    private Vector3Int currentCellEvent;

    ///////////////// Drag and Zoom
    private Vector3 lastMousePosition;
    private Vector3 currentMousePosition;
    private float originalOrthographicSize;
    private Vector3 cameraLimitBottomLeftCorner;
    private Vector3 cameraLimitTopRightCorner;

    private float mouseScrollSpeed = 6f;

    ///////////////// Current position
    public Vector3Int currentCell;
    private Tile selectionTile;
    private int selectionTileHeight = 5;
    private Vector3Int lastTile;

    void Start()
    {
        gameManager = GetComponent<GameManager>();
        tilemap = gameManager.tilemap;
        originalOrthographicSize = Camera.main.orthographicSize;
       
        BoundsInt bounds = tilemap.cellBounds;
        cameraLimitBottomLeftCorner = tilemap.CellToWorld(new Vector3Int(bounds.xMin, bounds.yMin, bounds.zMin));
        cameraLimitTopRightCorner = tilemap.CellToWorld(new Vector3Int(bounds.xMax - 1, bounds.yMax - 1, bounds.zMax - 1));

        selectionTile = gameManager.selectionTile;
        canvas = GameObject.Find("Canvas");
        menuPanel = GameObject.Find("Menu");
    }

    void Update()
    {
        //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (!cellEventPopupActive) { currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); currentMousePosition.z = 0; }
        currentCell = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        //Bail out if over UI
        hideY = 3 * Camera.main.orthographicSize / originalOrthographicSize;
        if (checkIfOverUI()) {deleteCursor(); return; }

        //Selection move
        if (gameManager.SelectionIsEnabled) moveCursor();
        else deleteCursor();

        //Select Tile
        if (gameManager.SelectionIsEnabled && Input.GetMouseButton(0)) mouseSelect(currentCell);

        //Hovering Event Popup over tile
        checkEventPopup(currentCell);
        if (cellEventPopupActive) return;

        //Drag map
        if (gameManager.DragIsEnabled && Input.GetMouseButton(1))
        {gameManager.currentAction = 0; gameManager.selectedConstruction = null; MouseDrag(currentMousePosition, lastMousePosition);}

        //Zoom in Map
        if (gameManager.ZoomIsEnabled && Input.GetAxis("Mouse ScrollWheel")!=0) MouseScroll(Input.GetAxis("Mouse ScrollWheel"));

        lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); lastMousePosition.z = 0;
    }

    public bool checkIfOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            canvas.GetComponent<GraphicRaycaster>().Raycast(ped, results);
            int count = results.Count;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "Fill" || result.gameObject.name == "PolutionBar" || result.gameObject.name == "EventPop-up" || result.gameObject.name == "EventPop-upText") count--;
            }
            if (count > 0) return true;
        }
        return false;
    }

    private void checkEventPopup(Vector3Int cell)
    {
        if (cellEventPopupActive == false)
        {
            CustomTile currentTile = gameManager.ReturnTileAbove(cell, 0);
            if (currentTile != null)
            {
                if (currentTile.type == CustomTile.Type.Event)
                {
                    //Instantiate Event Popup
                    eventPopup = Instantiate(eventPopupPrefab, canvas.transform);
                    Vector3 cleft = tilemap.CellToWorld(new Vector3Int(cell.x, cell.y + 1, 9));
                    Vector3 cright = tilemap.CellToWorld(new Vector3Int(cell.x + 1, cell.y + 1, 9));
                    eventPopup.transform.position = new Vector3((cleft.x + cright.x) / 2, cleft.y, cleft.z);
                    eventPopup.name = "EventPop-up";
                    eventPopup.GetComponentInChildren<TextMeshProUGUI>().text = currentTile.name;

                    currentCellEvent = cell;
                    cellEventPopupActive = true;
                }
            }
        }
        else if (currentCellEvent != cell)
        {
            Destroy(eventPopup);
            cellEventPopupActive = false;
        }
    }

    public void HideshowMenu(bool state)
    {
        if (panelUp)
        {
               menuPanel.transform.Translate(0, -hideY, 0);
            panelUp = false;
        }
        else
        {
            menuPanel.transform.Translate(0, hideY, 0);
            panelUp = true;
        }
    }

    private void mouseSelect(Vector3Int cell)
    {
        gameManager.MouseAction(cell);
        //voi folosi in forma asta pentru a-i spune gamemanagerului ce a fost selectat si poate acesta are nevoie sa primeasca mai multe selectii pe care le memoreaza
    }

    private void MouseDrag(Vector3 currentMousePosition, Vector3 lastMousePosition)
    {
        Vector3 limitBottomLeft = cameraLimitBottomLeftCorner;
        Vector3 limitTopRight = cameraLimitTopRightCorner;
        var top = Camera.main.orthographicSize;
        var left = top * Camera.main.aspect;
        limitBottomLeft.x += left;
        limitBottomLeft.y += top;
        limitTopRight.x -= left;
        limitTopRight.y -= top;

        Vector3 diff = lastMousePosition - currentMousePosition;

        if (IsVectorInsideBox(Camera.main.transform.position + diff, limitBottomLeft, limitTopRight))
        {
            Camera.main.transform.Translate(diff);
        }
    }

    private void MouseScroll(float ScrollWheelChange)
    {
        float newOrthographicSize = Camera.main.orthographicSize;
        newOrthographicSize -= ScrollWheelChange * mouseScrollSpeed;
        float newWidthSize = newOrthographicSize * Camera.main.aspect;

        if (newOrthographicSize > 2.5f && newOrthographicSize <= originalOrthographicSize)
        {
            if (ScrollWheelChange > 0) Camera.main.orthographicSize = newOrthographicSize;
            else
            {
                if (Camera.main.transform.position.y + newOrthographicSize < cameraLimitTopRightCorner.y)
                    if (Camera.main.transform.position.y - newOrthographicSize > cameraLimitBottomLeftCorner.y)
                        if (Camera.main.transform.position.x + newWidthSize < cameraLimitTopRightCorner.x)
                            if (Camera.main.transform.position.x - newWidthSize > cameraLimitBottomLeftCorner.x)
                                Camera.main.orthographicSize = newOrthographicSize;

            }
        }
    }

    private void moveCursor()
    { 
        currentCell.z = selectionTileHeight;
        if (currentCell != lastTile)
        {
            tilemap.SetTile(currentCell, selectionTile);
            if (tilemap.GetTile(lastTile)==selectionTile) tilemap.SetTile(lastTile, null);
            lastTile = currentCell;
        }
    }

    public void deleteCursor()
    {
        if (lastTile!=null) tilemap.SetTile(lastTile, null);
    }

    bool IsVectorInsideBox(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        if ((v2.x <= v1.x && v1.x <= v3.x) || (v2.x >= v1.x && v1.x >= v3.x))
            if ((v2.y <= v1.y && v1.y <= v3.y) || (v2.y >= v1.y && v1.y >= v3.y))
                return true;
        return false;
    }

    public void CreatePopupWindow()
    {
        GameObject w = Instantiate(windowPrefab, canvas.transform);
        windowList.Add(w);
    }
}
