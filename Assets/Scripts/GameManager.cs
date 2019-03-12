using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    //////////////////////////////////////////////////// MAP INITIALIZATION
    public Tilemap tilemap;
    private BoundsInt bounds;

    /*
    z = 0 for water
    z = 1 for ground1
    z = 2 for ground2
    z = 3 for ground3
    z = 4 for selection tile accesory
    z = 5 for selection tile 
    z = 6 for buildings and events
    z = 7 for possible overlays1
    z = 8 for possible overlays2
    z = 9 for UI 
    */

    //////////////////////////////////////////////////// TILE DATABASE
    [Serializable] public class DatabaseItem
    { 
        [SerializeField] public CustomTile tile;
        public int instances;
        public List<Vector3Int> locations;
    }
    [Serializable]
    public class DatabaseItemBase : DatabaseItem
    {
        [SerializeField] public List<DatabaseItem> upgrades;
    }
    [Serializable]
    public class DatabaseItemEvent
    {
        [SerializeField] public CustomTile tile;
        public int instances;
        public List <Vector3Int> locations;
        
    }
    [Space]
    public List <DatabaseItemBase> tileDatabase;
    [Space]
    public List <DatabaseItemEvent> eventDatabase;
    [Space]

    //////////////////////////////////////////////////// UI OPTIONS
    public Tile selectionTile;
    public bool SelectionIsEnabled = true;
    public bool ZoomIsEnabled = true;
    public bool DragIsEnabled = true;

    private TabMenuScript tabMenuScript;
    
    //////////////////////////////////////////////////// GAME STATES
    public int currentAction = 1;
    /*
    Action 0 Idle 
    Action 1 Build selected
    Action 2 Destroy selected
    Action 3 Construction Upgrade / Event Menu selected
    Action 4 Events
    */
    public CustomTile selectedConstruction;
    public Vector3Int selectedCell; //-might have to add stack of selected cells
    //////////////////////////////////////////////////// GAME VARIABLES
    public int gameTime;

    public int polutionRate = 0;
    public int Polution = 0;
    public int PolutionLimit = 1000;

    public int revenue = 0;
    public int Budget = 0;



     







    void Start()
    {
        //////// MAP INITIALIZATION
        tilemap.CompressBounds();
        bounds = tilemap.cellBounds;

        //////// UI
        tabMenuScript = GameObject.Find("TabMenu").GetComponent<TabMenuScript>();

        //////// GAME VARIABLES
        InvokeRepeating("GameTimeUpdate", 0, 1);
    }

    public void GameTimeUpdate()
    {
        gameTime += 1;
    }

    void Update()
    {
        PolutionUpdate();
        BudgetUpdate();
    }


   


    /// Polution Update
    int lastPolutionTimeUpdate = 0;
    int polutionUpdateRate = 4;
    private void PolutionUpdate()
    {
        if (gameTime - lastPolutionTimeUpdate > polutionUpdateRate)
        {
            Polution += polutionRate;
            lastPolutionTimeUpdate = gameTime;
        }
    }

    /// Budget Update
    int lastBudgetTimeUpdate = 0;
    int budgetUpdateRate = 6;
    private void BudgetUpdate()
    {
        if (gameTime - lastBudgetTimeUpdate > budgetUpdateRate)
        {
            Budget += revenue;
            lastBudgetTimeUpdate = gameTime;
        }
    }


    public void MouseAction(Vector3Int cell)
    {
        selectedCell = cell;
        CustomTile tileAbove = ReturnTileAbove(cell, 0);

        switch (currentAction)
        {
            case 0:
                {
                    if (tileAbove.type == CustomTile.Type.Construction) //&& construction is upgradable
                    {
                        currentAction = 3;
                        tabMenuScript.selectUpgrade();
                    }
                    break;
                }
            case 1:
                {
                    if (tileAbove.type == CustomTile.Type.Ground  && selectedConstruction!=null) //gotta add if enough money
                    {
                        GetComponent<MouseController>().deleteCursor();

                        //sets and logs in the database
                        tilemap.SetTile(new Vector3Int(cell.x, cell.y, selectedConstruction.zPos), selectedConstruction);

                        //lets the tile do something specific at instantiation
                        selectedConstruction.OnInstantiate(new Vector3Int(cell.x, cell.y, selectedConstruction.zPos));


                    }
                    break;
                }
            case 2:
                {
                    if (tileAbove.type == CustomTile.Type.Construction)
                    {
                        //lets the tile do something before it destroyed
                        tileAbove.OnDelete(cell);

                        //sets and delogs in the database
                        tilemap.SetTile(new Vector3Int(cell.x, cell.y, 6), null); //shoul create return first height
                    }
                    break;
                }
            case 3:
                {
                    if (tileAbove.type == CustomTile.Type.Construction)
                    {
                        selectedConstruction = ReturnTileAbove(cell,0);
                    }
                    else
                    {
                        selectedConstruction = null;
                        currentAction = 0;
                    }
                    break;
                }
        }
    }

    public void selectUpgrade(CustomTile upgrade)
    {
        Vector3Int pos = new Vector3Int(selectedCell.x, selectedCell.y, upgrade.zPos);
        CustomTile construction = ReturnTileAbove(pos, 0);

        //if is allowed
        if (!UpgradeIsValid(construction,upgrade)) return;

        //delete old tile
        construction.OnDelete(pos);

        //instantiate the new one
        tilemap.SetTile(pos,upgrade);
        upgrade.OnInstantiate(pos);
    }

    public bool UpgradeIsValid(CustomTile construction, CustomTile upgrade)
    {
        foreach (DatabaseItemBase datItem in tileDatabase)
        {
            List<DatabaseItem> tileList = datItem.upgrades;
            if (datItem.tile == construction)
            {
                foreach (DatabaseItem datTile in tileList)
                {
                    if (datTile.tile == upgrade) return true;
                }
            }
            else
            {
                bool foundConstr = false;
                foreach (DatabaseItem datTile in tileList)
                {
                    if (foundConstr && datTile.tile == upgrade ) return true;
                    if (datTile.tile == construction) foundConstr = true;
                }
            }
        }
        return false;
    }

    public CustomTile ReturnTileAbove (Vector3Int cell, int position)
    {
        CustomTile aux = null;
        for (int z=8; z >= -1 && position >= 0;z--)
        {
            aux = tilemap.GetTile<CustomTile>(new Vector3Int(cell.x, cell.y, z));
            if (aux!=null)
                position--;
        }
        return aux;
    }

}
