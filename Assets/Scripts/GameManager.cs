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
        public bool unlocked;
        public Sprite picture;
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
    public CustomTile buildZoneTile;
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
    public float gameTime;

    [Serializable]
    public class Clock
    {
        public int day = 1;
        public int month = 1;
        public int year = 2019;
    };

    public Clock StartDate;
    public Clock EndDate;

    public Clock CurrentDate;

    public int TimeForClear = 10;
    public float TimePerBeat = 1;
    public Text date;


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
        InvokeRepeating("GameTimeUpdate", 0, 1); // de facut coroutine
        SetTimer();
        InvokeRepeating("DateIncrease", 0, TimePerBeat);
    }

    void Update()
    {
        PolutionUpdate();
        BudgetUpdate();


    }

    /// Game Time
    public void GameTimeUpdate()
    {
        gameTime += 1;
        if (gameTime >= TimeForClear)
            Debug.Log(CheckResults()); //window log win/lose
    }

    void SetTimer()
    {
        int numOfDays = DaysDifference(StartDate, EndDate);
        TimePerBeat = (float)TimeForClear / numOfDays;
    }

    int DaysDifference(Clock a, Clock b)
    {
        int Y = a.year;
        int M = a.month;
        int D = a.day;
        long adays = (1461 * (Y + 4800 + (M - 14) / 12)) / 4 + (367 * (M - 2 - 12 * ((M - 14) / 12))) / 12 - (3 * ((Y + 4900 + (M - 14) / 12) / 100)) / 4 + D - 32075;
        Y = b.year;
        M = b.month;
        D = b.day;
        long bdays = (1461 * (Y + 4800 + (M - 14) / 12)) / 4 + (367 * (M - 2 - 12 * ((M - 14) / 12))) / 12 - (3 * ((Y + 4900 + (M - 14) / 12) / 100)) / 4 + D - 32075;
        return (int)Mathf.Abs(bdays - adays);
    }

    public void DateIncrease()
    {
        CurrentDate.day++; // de facut algoritm

        bool isLeapYear = ((CurrentDate.year % 4 == 0) && (CurrentDate.year % 100 != 0)) || (CurrentDate.year % 400 == 0);
        if (CurrentDate.day == 32 && CurrentDate.month == 12)
        { CurrentDate.day = 1; CurrentDate.month = 1; CurrentDate.year++; }
        else if (CurrentDate.day == 32 && (CurrentDate.month == 1 || CurrentDate.month == 3 || CurrentDate.month == 5 || CurrentDate.month == 7 || CurrentDate.month == 8 || CurrentDate.month == 10 ))
        { CurrentDate.day = 1; CurrentDate.month++; }
        else if (CurrentDate.day == 31 && (CurrentDate.month == 4 || CurrentDate.month == 6 || CurrentDate.month == 9 || CurrentDate.month == 11))
        { CurrentDate.day = 1; CurrentDate.month++; }
        else if ((CurrentDate.day == 29 && CurrentDate.month == 2 && !isLeapYear )|| (CurrentDate.day == 30 && CurrentDate.month == 2 && isLeapYear))
        { CurrentDate.day = 1; CurrentDate.month++; }

        date.text = CurrentDate.day + "." + CurrentDate.month + "." + CurrentDate.year;
    }

    /// Polution Update
    float lastPolutionTimeUpdate = 0;
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
    float lastBudgetTimeUpdate = 0;
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
                    CustomTile tileAbove2 = ReturnTileAbove(cell, 1);
                    if (((tileAbove == buildZoneTile ) && (tileAbove2.type == CustomTile.Type.Ground) && (selectedConstruction != null))
                        ||( (selectedConstruction.displayName == "Pole" ) && IsPoleNearCity(cell) && (tileAbove.type == CustomTile.Type.Ground)))
                    {
                        if (Budget >= selectedConstruction.costForAction) //poate verificarea ar trebui sa fie facute de CustomTile
                        {
                            GetComponent<MouseController>().deleteCursor();

                            //sets and logs in the database
                            tilemap.SetTile(new Vector3Int(cell.x, cell.y, selectedConstruction.zPos), selectedConstruction);

                            //lets the tile do something specific at instantiation
                            selectedConstruction.OnInstantiate(new Vector3Int(cell.x, cell.y, selectedConstruction.zPos));

                            tabMenuScript.ShowBuildZone();
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
                    if ((tileAbove.type == CustomTile.Type.Construction) && (tileAbove.displayName != "City"))
                    {
                        //lets the tile do something before it destroyed
                        tileAbove.OnDelete(new Vector3Int(cell.x, cell.y, 6));

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

    public bool IsPoleNearCity(Vector3Int cell)
    {
        foreach (GameManager.DatabaseItemBase baseItem in tileDatabase)
        {
            if ((baseItem.tile.displayName != "City") && (baseItem.tile.displayName != "Pole")) continue;

            foreach (Vector3Int location in baseItem.locations)
            {
                if ((cell.x - location.x) * (cell.x - location.x) + (cell.y - location.y) * (cell.y - location.y) < 26)
                    return true;
            }

            foreach (GameManager.DatabaseItem item in baseItem.upgrades)
            {
                if ((baseItem.tile.displayName != "City") && (baseItem.tile.displayName != "Pole")) continue;

                foreach (Vector3Int location in item.locations)
                {
                    if ((cell.x - location.x) * (cell.x - location.x) + (cell.y - location.y) * (cell.y - location.y) < 26)
                        return true;
                }
            }
        }
        return false;
    }

    public void selectUpgrade(CustomTile upgrade)
    {
        Vector3Int pos = new Vector3Int(selectedCell.x, selectedCell.y, upgrade.zPos);
        CustomTile construction = ReturnTileAbove(pos, 0);

        //if is allowed
        if (!UpgradeIsValid(construction, upgrade) || upgrade.costForAction > Budget) return;

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
        eventDatabase.Remove(eventDatabase.Find(ev => ev.tile == customTile)); //trebuie sa sterg evenimentul - sa pun add cell in on instantiate?
        GameObject.Find("EventsHolder").GetComponent<ObjectHolderScript>().listUpToDate = false;
        tabMenuScript.selectEvents();
        return;
    }

    public bool CheckResults()
    {
        foreach (DatabaseItemRequirement req in requirementsDatabase)
        {
            if (!req.completed) return false;
        }
        return true;
    }
}