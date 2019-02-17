using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseController : MonoBehaviour
{
    private GameManager gameManager;

    private float originalOrthographicSize;
    Vector3 cameraLimitBottomLeftCorner;
    Vector3 cameraLimitTopRightCorner;

    private Vector3 lastMousePosition;
    private Vector3 currentMousePosition;
    private float mouseScrollSpeed = 6f;

    public Tile highlightTile;
    private Vector3Int lastTile;

    void Start()
    {
        gameManager = GetComponent<GameManager>();
        originalOrthographicSize = Camera.main.orthographicSize;

        cameraLimitBottomLeftCorner = gameManager.tilemap.CellToWorld(new Vector3Int(gameManager.bounds.xMin, gameManager.bounds.yMin, 0));
        cameraLimitTopRightCorner = gameManager.tilemap.CellToWorld(new Vector3Int(gameManager.bounds.xMax - 1, gameManager.bounds.yMax - 1, 0));
    }

    void Update()
    {
        currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMousePosition.z = 0;

        //Drag map
        if (Input.GetMouseButton(1))
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
            
            if (IsVectorInsideBox(Camera.main.transform.position + diff,limitBottomLeft,limitTopRight))
            {
                Camera.main.transform.Translate(diff);
            }
        }

        //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        //Zoom in Map
        float ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");
        if (gameManager.ZoomIsEnabled && (ScrollWheelChange != 0))
        {

            float newOrthographicSize = Camera.main.orthographicSize;
            newOrthographicSize -= ScrollWheelChange * mouseScrollSpeed;
            float newWidthSize = newOrthographicSize * Camera.main.aspect;

            if (newOrthographicSize > 0 && newOrthographicSize <= originalOrthographicSize) {
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

        //Selection move
        if (gameManager.SelectionIsEnabled) moveCursor();

        lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastMousePosition.z = 0;
    }

    private void moveCursor()
    {
        Vector3Int currentTile = gameManager.tilemap.WorldToCell(currentMousePosition);
        currentTile.z = gameManager.selectionTileHeight;
        if (currentTile != lastTile)
        {
            gameManager.tilemap.SetTile(currentTile, highlightTile);
            gameManager.tilemap.SetTile(lastTile, null);
            lastTile = currentTile;
        }
    }

    bool IsVectorInsideBox(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        if ((v2.x <= v1.x && v1.x <= v3.x) || (v2.x >= v1.x && v1.x >= v3.x))
            if ((v2.y <= v1.y && v1.y <= v3.y) || (v2.y >= v1.y && v1.y >= v3.y))
                return true;
        return false;
    }
}
