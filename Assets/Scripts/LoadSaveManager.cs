using System;
using UnityEngine;
using System.IO;
using UnityEditor;

public class LoadSaveManager : MonoBehaviour
{
    private DungeonGen dungeonGen;
    private TileGen tileGen;

    private string filePath;
    private string fileLocation;


    private void Awake()
    {
        tileGen = GetComponent<TileGen>();
        dungeonGen = GetComponent<DungeonGen>();

        fileLocation = "MapData/";
    }


    public void SaveLevel()
    {
        if (tileGen.GetTileMap() != null)
        {
            // Get TimeStamp for level save
            string filename = "mapData" + DateTime.Now.ToString("(hhmmss)(ddMMyyyy)") + ".json";

            // Update filePath
            filePath = Path.Combine(Application.dataPath + "/Resources/MapData", filename);

            // Get data from mapData and save
            string jsonString = JsonUtility.ToJson(GenerateSaveData());

            // Write to JSON
            File.WriteAllText(filePath, jsonString);

            AssetDatabase.Refresh();

            Debug.Log("Level Saved");
        }

        else
            Debug.Log("Unable to Save, no data available");

    }


    public void LoadLevel()
    {
        /*if(!dungeonGen.FirstMap())
        {
            dungeonGen.ResetMap();
        }

        // Read data from selected level and generate
        GenerateLoadData();

        dungeonGen.SetupLoadedLevel();*/

        // Initialise loaded level

        // Get the name of each level in save folder (Only retreive json files!
        foreach (string file in System.IO.Directory.
            GetFiles("Assets/Resources/MapData/" + "/", "*.json"))
        {
            Debug.Log(file);
        }

    }


    public void GenerateLevelList()
    {
        // generate a list of saved levels

    }


    private LevelData GenerateSaveData()
    {
        LevelData data = new LevelData();

        // What do we want to save?
        // Map height and width
        // is a tile walkable? (ie a dungeon tile or not)
        foreach (Tile tile in tileGen.GetTileMap())
        {
            if (tile.IsWalkable())
            {
                data.tileTypes.Add(0);
                continue;
            }

            data.tileTypes.Add(-1);
        }

        data.mapWidth  = tileGen.GetMapWidth();
        data.mapHeight = tileGen.GetMapHeight();

        return data;
    }


    private void GenerateLoadData()
    {
        LevelData data = new LevelData();

        TextAsset asset = Resources.Load<TextAsset>(fileLocation + "Level");

        if (asset != null)
        {
            data = JsonUtility.FromJson<LevelData>(asset.text);

            tileGen.LoadTileMap(data);

            Debug.Log("Map Loaded");
        }

        else
            Debug.Log("Asset is Null");

        //return map;
    }
}
