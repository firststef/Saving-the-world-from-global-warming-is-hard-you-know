using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //////////////////////////////////////////////////// MAP INITIALIZATION
    public MouseController mouseController;
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

    //////////////////////////////////////////////////// DATABASE
    [Serializable]
    public class DatabaseItem
    {
        [SerializeField] public CustomTile tile;
        public int instances;
        public List<Vector3Int> locations;
    };
    [Serializable]
    public class DatabaseItemBase : DatabaseItem
    {
        [SerializeField] public List<DatabaseItem> upgrades;
    };
    [Serializable]
    public class DatabaseItemEvent
    {
        [SerializeField] public CustomTile tile;
        public int instances;
        public List<Vector3Int> locations;
    };
    [Serializable]
    public class DatabaseItemRequirement
    {
        [SerializeField] public CustomTile tile;
        public string variable;
        public string title;
        public string description;
        public int amount;
        public bool completed = false;
    };
    [Space]
    public List<DatabaseItemBase> tileDatabase;
    [Space]
    public List<DatabaseItemEvent> eventDatabase;
    [Space]
    public List<DatabaseItemRequirement> requirementsDatabase;
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
    Action 1 Requirements
    Action 2 Build selected
    Action 3 Destroy selected
    Action 4 Construction Upgrade / Event Menu selected
    Action 5 Events
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
        //////// INITIALIZATION
        mouseController = GetComponent<MouseController>();

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
        gameTime += 1;// de facut o functie care converteste gametime in zile
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
                        tabMenuScript.selectUpgrade();
                    }
                    break;
                }
            case 1:
                break;
            case 2:
                {
                    if (tileAbove.type == CustomTile.Type.Ground && selectedConstruction != null)
                    {
                        if (Budget >= selectedConstruction.costForAction) //poate verificarea ar trebui sa fie facute de CustomTile
                        {
                            GetComponent<MouseController>().deleteCursor();

                            //sets and logs in the database
                            tilemap.SetTile(new Vector3Int(cell.x, cell.y, selectedConstruction.zPos), selectedConstruction);

                            //lets the tile do something specific at instantiation
                            selectedConstruction.OnInstantiate(new Vector3Int(cell.x, cell.y, selectedConstruction.zPos));
                        }
                        else
                        {
                            mouseController.CreatePopupWindow();
                        }
                    }
                    break;
                }
            case 3:
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
            case 4:
                {
                    if (tileAbove.type == CustomTile.Type.Construction)
                    {
                        selectedConstruction = ReturnTileAbove(cell, 0);
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
        if (!UpgradeIsValid(construction, upgrade) && upgrade.costForAction > Budget) return;

        //delete old tile
        construction.OnDelete(pos);

        //instantiate the new one
        tilemap.SetTile(pos, upgrade);
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
                    if (foundConstr && datTile.tile == upgrade) return true;
                    if (datTile.tile == construction) foundConstr = true;
                }
            }
        }
        return false;
    }

    public CustomTile ReturnTileAbove(Vector3Int cell, int position)
    {
        CustomTile aux = null;
        for (int z = 8; z >= -1 && position >= 0; z--)
        {
            aux = tilemap.GetTile<CustomTile>(new Vector3Int(cell.x, cell.y, z));
            if (aux != null)
                position--;
        }
        return aux;
    }
     
    public int retVariableForRequirement(string name)
    {
        switch (name)
        {
            case "Budget":
                    return Budget;
            case "Revenue":
                return revenue;
        }
        return 0;
    }

    public int retVariableForRequirement(CustomTile tile)
    {
        foreach (DatabaseItemBase databaseItem in tileDatabase)
        {
            if (databaseItem.tile == tile)
            {
                return databaseItem.instances;
            }
            foreach (DatabaseItem tileu in databaseItem.upgrades)
            {
                if (tileu.tile == tile)
                {
                    return tileu.instances;
                }
            }
        }
        return 0;
    }

    public void ResolveEventOption (CustomTile customTile, int button)
    {
        int ok = 1;

        List<CustomTile.Demand> listDem = null;
        if (button == 1)
            listDem = customTile.demands1;
        else if (button == 2)
            listDem = customTile.demands2;
        else if (button == 3)
            goto skip3;
        foreach (CustomTile.Demand demand in listDem)
        {
            int stock = 0;
            if (demand.ctile != null) stock = retVariableForRequirement(demand.ctile);
            else if (demand.variable != null) stock = retVariableForRequirement(demand.variable);
            else return; 

            if (stock < demand.amount) ok = 0;
        }

        if (ok == 0) return; //window log not enough suplies

        foreach (CustomTile.Demand demand in listDem)
        {
            if (demand.ctile != null)
            {
                int newValue = retVariableForRequirement(demand.ctile) - demand.amount;

                foreach (DatabaseItemBase databaseItem in tileDatabase)
                {
                    if (databaseItem.tile == demand.ctile)
                    {
                        for (int i = databaseItem.instances-1; i>= 0 && i>=newValue; i--)
                        {
                            databaseItem.tile.OnDelete(databaseItem.locations[i]);
                        }
                    }
                    foreach (DatabaseItem tileu in databaseItem.upgrades)
                    {
                        for (int i = tileu.instances - 1; i >= 0 && i>=newValue; i--)
                        {
                            tileu.tile.OnDelete(tileu.locations[i]);
                        }
                    }
                }
            }
            else
            {
                switch (demand.variable) //ar merge o functie de optimizare aici ceva gen ret_variable (string, modify 0);
                {
                    case "Budget":
                        Budget += demand.amount;
                        break;
                    case "Revenue":
                        revenue += demand.amount;
                        break;
                }
            }
        }

        skip3:
        eventDatabase.Remove(eventDatabase.Find(ev => ev.tile == customTile));
        GameObject.Find("EventsHolder").GetComponent<ObjectHolderScript>().listUpToDate = false;
        tabMenuScript.selectEvents();
        return;
    }
}