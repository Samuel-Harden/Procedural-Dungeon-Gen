using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadSaveManager : MonoBehaviour
{
    [SerializeField]TileGen tileGen;

    private string filePath;


    private void Awake()
    {
        tileGen = GetComponent<TileGen>();

        filePath = Path.Combine(Application.dataPath, "Level.json");
    }


    public void SaveLevel()
    {
        // Get data from mapData and save
        string jsonString = JsonUtility.ToJson(GenerateSaveData());

        // Write to JSON
        File.WriteAllText(filePath, jsonString);

        Debug.Log("Level Saved");

    }


    public void LoadLevel()
    {
        // Read data from selected level and generate
        GenerateLoadData();

        // Initialise loaded level

    }


    public void GenerateLevelList()
    {
        // generate a list of saved levels

    }


    private Map GenerateSaveData()
    {
        Map map = new Map();

        // What do we want to save?
        // Map height and width
        // is a tile walkable? (ie a dungeon tile or not)

        foreach (Tile tile in tileGen.GetTileMap())
        {
            if (tile.IsWalkable())
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


    private void GenerateLoadData()
    {
        Map map = new Map();

        TextAsset asset = Resources.Load<TextAsset>("Level");

        if (asset != null)
        {
            map = JsonUtility.FromJson<Map>(asset.text);
        }

        else
            Debug.Log("Asset is Null");

        //return map;
        Debug.Log("Map Loaded");
    }
}
