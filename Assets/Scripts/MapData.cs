using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    private TileGen tileGen;
    public Map myMap;


    private void Start()
    {
        tileGen = GetComponent<TileGen>();

        GenerateLoadData();
    }


    public Map GenerateSaveData()
    {
        Map map = new Map();

        // What do we want to save?
        // Map height and width
        // is a tile walkable? (ie a dungeon tile or not)

        foreach (Tile tile in tileGen.GetTileMap())
        {
            if(tile.IsWalkable())
            {
                map.tileTypes.Add(0);
                continue;
            }

            map.tileTypes.Add(-1);
        }

        map.mapWidth = tileGen.GetMapWidth();
        map.mapHeight = tileGen.GetMapHeight();

        return map;
    }


    public void GenerateLoadData()
    {
        //Map map = new Map();

        TextAsset asset = Resources.Load<TextAsset>("Level");

        if (asset != null)
        {
            myMap = JsonUtility.FromJson<Map>(asset.text);
        }

        else
            Debug.Log("Asset is Null");

        //return map;
        Debug.Log("Map Loaded");
    }
}
