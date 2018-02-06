using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneration : MonoBehaviour
{
    [SerializeField] LayerMask checkLayers;

    [SerializeField] GameObject tilePrefab;

    private GameObject[,] tileMap;

    int minPosX;
    int maxPosX;
    int minPosZ;
    int maxPosZ;

    int tileSize = 1;

    int mapWidth;
    int mapHeight;


    public void Initialize(List<Room> _rooms, Vector3 _dungeonCentre)
    {
        // Checklist
        // 1) Get Max min and max Pos of dungeon
        // 2) Generate a tilebased map

        // Set initial Min & max to == dungeonCentre
        minPosX = (int)_dungeonCentre.x;
        maxPosX = (int)_dungeonCentre.x;

        minPosZ = (int)_dungeonCentre.z;
        maxPosZ = (int)_dungeonCentre.z;

        GetDungeonBounds(_rooms);

        GenerateTileMap();

        CleanUpRooms(_rooms);
    }


    private void GenerateTileMap()
    {
        mapWidth = maxPosX - minPosX / tileSize;
        mapHeight = maxPosZ - minPosZ / tileSize;

        tileMap = new GameObject[mapWidth, mapHeight];

        Vector3 checkPos = new Vector3(minPosX + (float)tileSize / 2, 0.0f, minPosZ + (float)tileSize / 2);

        Vector3 spawnPos = new Vector3((float)tileSize / 2, 0.0f, (float)tileSize / 2);

        Debug.Log(checkPos);

        for (int h = 0; h < mapHeight; h++)
        {
            for (int w = 0; w < mapWidth; w++)
            {
                bool dungeon = (Physics.CheckSphere(checkPos, ((float)tileSize / 3), checkLayers));

                if(dungeon)
                {
                    var tile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);

                    tileMap[w, h] = tile;

                    //tile.transform.position = spawnPos;

                    tile.transform.localScale = Vector3.one;
                }

                spawnPos.x += tileSize;
                checkPos.x += tileSize;

                Debug.Log(checkPos);
            }

            spawnPos.x = (float)tileSize / 2;
            spawnPos.z += tileSize;

            checkPos.x = minPosX + (float)tileSize / 2;
            checkPos.z += tileSize;
        }
    }


    private void CleanUpRooms(List<Room> _rooms)
    {
        foreach(Room room in _rooms)
        {
            Destroy(room.gameObject);
        }
    }


    private void GetDungeonBounds(List<Room> _rooms)
    {
        SetLowestXPos (_rooms);
        SetHighestXPos(_rooms);
        SetLowestZPos (_rooms);
        SetHighestZPos(_rooms);
    }


    private void SetLowestXPos(List<Room> _rooms)
    {
        // Find Lowest X position
        foreach (Room room in _rooms)
        {
            int pos = (int)room.GetRoomBoundsPoint().x;

            if (pos < minPosX)
                minPosX = pos;
        }
    }


    private void SetHighestXPos(List<Room> _rooms)
    {
        // Find highest X position
        foreach (Room room in _rooms)
        {
            int pos = (int)room.GetRoomBoundsPoint().x + (int)room.GetRoomWidth();

            if (pos > maxPosX)
                maxPosX = pos;
        }
    }


    private void SetLowestZPos(List<Room> _rooms)
    {
        // Find Lowest Z position
        foreach (Room room in _rooms)
        {
            int pos = (int)room.GetRoomBoundsPoint().z;

            if (pos < minPosZ)
                minPosZ = pos;
        }
    }


    private void SetHighestZPos(List<Room> _rooms)
    {
        // Find highest Z position
        foreach (Room room in _rooms)
        {
            int pos = (int)room.GetRoomBoundsPoint().z + (int)room.GetRoomHeight();

            if (pos > maxPosZ)
                maxPosZ = pos;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        /*Gizmos.DrawWireCube(new Vector3(((float)maxPosX + (float)minPosX) / 2, 0.0f,
            ((float)maxPosZ + (float)minPosZ) / 2),
            new Vector3((float)maxPosX - (float)minPosX, 0.0f,
            (float)maxPosZ - (float)minPosZ));*/

        Gizmos.DrawWireCube(new Vector3((float)mapWidth / 2, 0.0f, (float)mapHeight / 2), new Vector3((float)mapWidth, 0.0f, (float)mapHeight));
    }
}
