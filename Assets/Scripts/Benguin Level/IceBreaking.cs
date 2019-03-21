using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class IceBreaking : MonoBehaviour
{
    private Tilemap tilemap;
    public GameObject player;
    public Tile IceTile;
    public Tile[] BrokenIceTiles;
    private float timer;

    private Vector3Int prevCell;
    private Vector3Int currentCell;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        if (tilemap.name != "Ice") Destroy(this);

        prevCell = tilemap.WorldToCell(player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 feetPosition = player.transform.position;
        feetPosition.y -= (float)0.48;
        Vector3Int currentCell = tilemap.WorldToCell(feetPosition);
        currentCell.z = 0;
        if (currentCell != prevCell)
        {
            if (tilemap.GetTile(prevCell) == IceTile && tilemap.GetTile(prevCell) != null) StartCoroutine(BreakAnimation(prevCell));
            prevCell = currentCell;
        }
    }

    private IEnumerator BreakAnimation(Vector3Int cell)
    {
        Tile tile;
        for (int i = 0; i < BrokenIceTiles.Length; i++)
        {
            tile = BrokenIceTiles[i];
            tilemap.SetTile(cell, tile);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.2f);
        tilemap.SetTile(cell, null); // unless the tile is a special tile
    }

}
