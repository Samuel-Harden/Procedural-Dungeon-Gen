using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadSaveManager : MonoBehaviour
{
    MapData mapData;

    private string filePath;


    private void Start()
    {
        mapData = GetComponent<MapData>();

        filePath = Path.Combine(Application.dataPath, "Level.json");
    }


    public void SaveLevel()
    {
        // Get data from mapData and save
        string jsonString = JsonUtility.ToJson(mapData.GenerateSaveData());
        File.WriteAllText(filePath, jsonString);

        Debug.Log("Level Saved");

    }


    public void LoadLevel()
    {
        // Read data from selected level and generate
        mapData.GenerateLoadData();
    }


    public void GenerateLevelList()
    {
        // generate a list of saved levels

    }
}
