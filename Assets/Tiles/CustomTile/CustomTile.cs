using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps
{

[Serializable]
[CreateAssetMenu(fileName = "New Custom Tile", menuName = "Tiles/Custom Tile")]


    public class CustomTile : Tile
    {
        public int zPos = 0;

        [Serializable] public enum Type
        {
            Undefined,
            Ground,
            Construction,
            Event
        }
        public Type type;

        public int polutionNumber = 0;
        public int revenueNumber = 0;

        private GameManager gameManager=null;

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
                        
                            datItem.upgrades.Find(dT => dT.tile == this).instances++;
                            datItem.upgrades.Find(dT => dT.tile == this).locations.Add(cell);

                    }
                }
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
                    if (this == datItem.tile)
                    {
                        datItem.instances--;
                        datItem.locations.Remove(cell);
                    }
                    else
                    {
                         
                        datItem.upgrades.Find(dT => dT.tile == this).instances--;
                        datItem.upgrades.Find(dT => dT.tile == this).locations.Remove(cell);
                      
                    }
                }
            }
            else if (type == Type.Event)
            {
                GameObject.Find("EventsHolder").GetComponent<ObjectHolderScript>().listUpToDate = false;
                GameManager.DatabaseItemEvent datItem = gameManager.eventDatabase.Find(DatabaseItem => DatabaseItem.tile.name == name);//.instances++;
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

        tile.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite ", tile.sprite, typeof(Sprite), false, null);

        int zPos = (int)EditorGUILayout.FloatField("Z Position", tile.zPos);
        tile.zPos = zPos;

        EditorGUILayout.Space();
        CustomTile.Type type = (CustomTile.Type)EditorGUILayout.EnumPopup("Type", tile.type);
        tile.type = type;

        switch (type)
        {
             case CustomTile.Type.Construction:
                 {
                      int polution = (int)EditorGUILayout.IntField("Polution Number",tile.polutionNumber);
                      tile.polutionNumber = polution;

                        int revenue = (int)EditorGUILayout.IntField("Revenue Number", tile.revenueNumber);
                        tile.revenueNumber = revenue;
                        break;
                 }
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(tile);
    }
}
#endif
}