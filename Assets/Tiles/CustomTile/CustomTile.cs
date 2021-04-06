using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using static UnityEngine.Tilemaps.CustomTile;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps
{

[Serializable]
[CreateAssetMenu(fileName = "New Custom Tile", menuName = "Tiles/Custom Tile")]


    public class CustomTile : Tile
    {
        private GameManager gameManager = null;

        public string displayName;
        public int zPos = 0;

        [Serializable] public enum Type
        {
            Undefined,
            Ground,
            BuildZone,
            Construction,
            Event,
            Opportunity //am nevoie de oportunity pentru a sti sa nu instantiez tile-ul in event database si sa il sterg dupa fara adresa dar sunt acelasi lucru in mare
        }
        public Type type;

        public string description;

        ////////////Constructions
        public int polutionNumber = 0;
        public int revenueNumber = 0;

        public int costForAction = 0;

        ////////////Events and Oportunities
        [Serializable]
        public class Demand
        {
            public string variable;
            public CustomTile ctile = null;
            public int amount;
        };

        public string button1;

        public List<Demand> demands1 = new List<Demand>(0);

        public string button2;

        public List<Demand> demands2 = new List<Demand>(0);

        public string button3;

        public void OnInstantiate(Vector3Int cell)
        {
            Debug.Log("Tile initialized");
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            if (type == Type.Construction)
            {
                gameManager.polutionRate += this.polutionNumber;
                gameManager.revenue += this.revenueNumber;
            }

            if (type == Type.Construction)
            {
                foreach (GameManager.DatabaseItemBase datItem in gameManager.tileDatabase)
                {
                    if (this == datItem.tile)
                    {
                        datItem.instances++;
                        datItem.locations.Add(cell);
                    }
                    else
                    {

                        GameManager.DatabaseItem up = datItem.upgrades.Find(dT => dT.tile == this);
                        if (up != null)
                        {
                            up.instances++;
                            up.locations.Add(cell);
                        }

                    }
                }
                gameManager.Budget -= costForAction;
            }
            else if (type == Type.Event)
            {
                GameObject.Find("EventsHolder").GetComponent<ObjectHolderScript>().listUpToDate = false;
                GameManager.DatabaseItemEvent datItem = gameManager.eventDatabase.Find(DatabaseItem => DatabaseItem.tile.name == name);//.instances++;
                if (datItem == null)
                {
                    GameManager.DatabaseItemEvent newEvent = new GameManager.DatabaseItemEvent() ;
                    newEvent.tile = this;
                    newEvent.locations = new List<Vector3Int>();
                    newEvent.locations.Add(cell);
                    newEvent.instances = 1;
                    gameManager.eventDatabase.Add(newEvent);
                }
                else
                {
                    gameManager.eventDatabase.Find(DatabaseItem => DatabaseItem.tile.name == name).instances++;
                    gameManager.eventDatabase.Find(DatabaseItem => DatabaseItem.tile.name == name).locations.Add(cell);
                }
            }
        }

        public void OnDelete(Vector3Int cell)
        {
            Debug.Log("Tile destroyed");
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (type == Type.Construction)
            {
                gameManager.polutionRate -= this.polutionNumber;
                gameManager.revenue -= this.revenueNumber;
            }

            if (type == Type.Construction)
            {
                foreach (GameManager.DatabaseItemBase datItem in gameManager.tileDatabase)
                {
                    if (datItem.tile == this)
                    {
                        datItem.instances--;
                        datItem.locations.Remove(cell);
                    }
                    else
                    {
                        GameManager.DatabaseItem up = datItem.upgrades.Find(dT => dT.tile == this);
                        if (up != null)
                        {
                            up.instances--;
                            up.locations.Remove(cell);
                        }

                    }
                }
            }
            else if (type == Type.Event)
            {
                GameObject.Find("EventsHolder").GetComponent<ObjectHolderScript>().listUpToDate = false;
                GameManager.DatabaseItemEvent datItem = gameManager.eventDatabase.Find(DatabaseItem => DatabaseItem.tile.name == name);
                if (datItem != null)
                {
                    if (datItem.instances == 1)
                    {
                        gameManager.eventDatabase.Remove(datItem);
                    }
                    else
                    {
                        gameManager.eventDatabase.Find(DatabaseItem => DatabaseItem.tile.name == name).instances--;
                        gameManager.eventDatabase.Find(DatabaseItem => DatabaseItem.tile.name == name).locations.Remove(cell);
                    }
                }
            }
        }

        public void OnEventButton ()
        {
            //
        }

        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(location, tilemap, ref tileData);
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(CustomTile))]
    public class CustomTileEditor : Editor
    {
        private CustomTile tile { get { return (target as CustomTile); } }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            tile.displayName= (string)EditorGUILayout.TextField("Display Name", tile.displayName);

            tile.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite ", tile.sprite, typeof(Sprite), false, null);

            tile.description = (string)EditorGUILayout.TextField("Description", tile.description);

            int zPos = (int)EditorGUILayout.FloatField("Z Position", tile.zPos);
            tile.zPos = zPos;

            EditorGUILayout.Space();
            CustomTile.Type type = (CustomTile.Type)EditorGUILayout.EnumPopup("Type", tile.type);
            tile.type = type;
            EditorGUILayout.Space();
            switch (type)
            {
                case CustomTile.Type.Construction:
                    {
                        int polution = (int)EditorGUILayout.IntField("Polution Number", tile.polutionNumber);
                        tile.polutionNumber = polution;

                        int revenue = (int)EditorGUILayout.IntField("Revenue Number", tile.revenueNumber);
                        tile.revenueNumber = revenue;

                        int cost = (int)EditorGUILayout.IntField("Cost For Action", tile.costForAction);
                        tile.costForAction = cost;
                        break;
                    }
                case CustomTile.Type.Event:
                case CustomTile.Type.Opportunity:
                    {
                        //////// Button1

                        tile.button1 = (string)EditorGUILayout.TextField("Button 1", tile.button1);

                        int count1 = EditorGUILayout.DelayedIntField("Number of demands", tile.demands1.Count != 0 ? tile.demands1.Count : 0);
                        if (count1 < 0)
                            count1 = 0;

                        if (tile.demands1.Count == 0 || tile.demands1.Count != count1)
                        {
                            //tile.demands1 = new List<Demand>(count1);
                            int cur = tile.demands1.Count;
                            if (count1 < cur)
                                tile.demands1.RemoveRange(count1, cur - count1);
                            else if (count1 > cur)
                            {
                                tile.demands1.AddRange(new Demand[count1 - cur]);
                            }
                        }

                        if (count1 == 0)
                            goto skip1;

                        for (int i = 0; i < count1; i++)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Element "+i);
                            tile.demands1[i].variable = (string)EditorGUILayout.TextField("Variable", tile.demands1[i].variable);
                            tile.demands1[i].ctile = (CustomTile)EditorGUILayout.ObjectField("Tile", tile.demands1[i].ctile, typeof(CustomTile), false, null);
                            tile.demands1[i].amount = (int)EditorGUILayout.IntField("Amount", tile.demands1[i].amount);
                            EditorGUILayout.Space();
                        }
                        
                        skip1:
                        EditorGUILayout.Space();
                        //////// Button2

                        tile.button2 = (string)EditorGUILayout.TextField("Button 2", tile.button2);

                        int count2 = EditorGUILayout.DelayedIntField("Number of demands", tile.demands2.Count != 0 ? tile.demands2.Count : 0);
                        if (count2 < 0)
                            count2 = 0;

                        if (tile.demands2.Count == 0 || tile.demands2.Count != count1)
                        {
                            //tile.demands1 = new List<Demand>(count1);
                            int cur = tile.demands2.Count;
                            if (count2 < cur)
                                tile.demands2.RemoveRange(count2, cur - count2);
                            else if (count2 > cur)
                            {
                                if (count2 > tile.demands2.Capacity)//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                                    tile.demands2.Capacity = count2;
                                tile.demands1.AddRange(new Demand[count2 - cur]);
                            }
                        }

                        if (count2 == 0)
                            goto skip2;

                        for (int i = 0; i < count2; i++)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Element " + i);
                            tile.demands2[i].variable = (string)EditorGUILayout.TextField("Variable", tile.demands2[i].variable);
                            tile.demands2[i].ctile = (CustomTile)EditorGUILayout.ObjectField("Tile", tile.demands2[i].ctile, typeof(CustomTile), false, null);
                            tile.demands2[i].amount = (int)EditorGUILayout.IntField("Amount", tile.demands2[i].amount);
                            EditorGUILayout.Space();
                        }
                        
                        skip2:
                        EditorGUILayout.Space();
                        //////// Button3
                        tile.button3 = (string)EditorGUILayout.TextField("Button 3", tile.button3);
                        break;
                    }
            }
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(tile);
        }

        
    }
#endif
}