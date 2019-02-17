using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tilemap tilemap;
    public int selectionTileHeight = 3;
    public BoundsInt bounds;
    public int mapHeight;
    public int mapWidth;

    public bool SelectionIsEnabled = true;
    public bool ZoomIsEnabled = true;

    void Awake()
    {
        tilemap.CompressBounds();
        bounds = tilemap.cellBounds;
        mapHeight = (int)Camera.main.ScreenToWorldPoint(bounds.size).x;
        mapWidth = (int)Camera.main.ScreenToWorldPoint(bounds.size).y;
        //Debug.Log(mapHeight);
        //Debug.Log(Camera.main.orthographicSize*2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
