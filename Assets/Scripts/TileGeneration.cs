using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneration : MonoBehaviour
{
    [SerializeField] GameObject tileContainer;

    [SerializeField] GameObject tilePrefab;

    private GameObject[,] tileMap;

    private int minPosX;
    private int maxPosX;
    private int minPosZ;
    private int maxPosZ;

    private int tileSize = 1;

    private int mapWidth;
    private int mapHeight;

    public void Initialize(List<Room> _rooms, Vector3 _dungeonCentre)
    {
        minPosX = (int)_dungeonCentre.x;
        maxPosX = (int)_dungeonCentre.x;

        minPosZ = (int)_dungeonCentre.z;
        maxPosZ = (int)_dungeonCentre.z;

        GetDungeonBounds(_rooms);

        GenerateTileMap(_rooms);

        //CleanUpRooms(_rooms);
    }


    private void GenerateTileMap(List<Room> _rooms)
    {
        mapWidth = maxPosX - minPosX / tileSize;
        mapHeight = maxPosZ - minPosZ / tileSize;

        mapWidth += 2;
        mapHeight += 2;

        tileMap = new GameObject[mapHeight, mapWidth];

        Color roomCol = Color.white;

        // Add each room pos to tile grid
        foreach (Room room in _rooms)
        {
            // Set Start point
            int posX = (int)room.GetRoomBoundsPoint().x + 1;
            int posZ = (int)room.GetRoomBoundsPoint().z + 1;

            int spawnPosX = posX - minPosX;
            int spawnPosZ = posZ - minPosZ;

            // First, add in all rooms to tile map
            for (int h = 0; h < room.GetHeight(); h++)
            {
                for (int w = 0; w < room.GetWidth(); w++)
                {
                    // Do we want to keep this room? (if its a small room?)
                    if(room.GenerateTile())
                    {
                        if (room.GetRoomType() > 0)
                            roomCol = Color.grey;

                        tileMap[posZ - minPosZ, posX - minPosX] = 
                            CreateTile(spawnPosX, spawnPosZ, room.GetRoomID(),
                            posX, posZ, roomCol);

                        posX += tileSize;

                        spawnPosX = posX - minPosX;
                    }
                }

                posX = (int)room.GetRoomBoundsPoint().x + 1;
                spawnPosX = posX - minPosX;
                posZ += tileSize;
                spawnPosZ = posZ - minPosZ;
            }
        }

        // now loop through any empty spaces, add in blanks (Walls)
        for (int h = 0; h < mapHeight; h++)
        {
            for (int w = 0; w < mapWidth; w++)
            {
                // If tile is not a room create empty tile
                if(tileMap[h,w] == null)
                {
                    roomCol = Color.black;

                    tileMap[h, w] = CreateTile(w, h, -1, w, h, roomCol);
                }
            }
        }
    }


    private GameObject CreateTile(int _spawnPosX, int _spawnPosZ, int _roomID,
        int _posX, int _posZ, Color _roomCol)
    {
        Color roomCol = _roomCol;

        var tile = Instantiate(tilePrefab, new Vector3(_spawnPosX, 0, _spawnPosZ),
            Quaternion.identity);

        tile.GetComponent<Tile>().SetData(_posX, _posZ, _roomID);

        tile.GetComponent<Renderer>().material.color = roomCol;

        tile.transform.SetParent(tileContainer.transform);

        return tile;
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
            int pos = (int)room.GetRoomBoundsPoint().x + (int)room.GetWidth();

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
            int pos = (int)room.GetRoomBoundsPoint().z + (int)room.GetHeight();

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

        //Gizmos.DrawWireCube(new Vector3((float)mapWidth / 2, 0.0f,
        //(float)mapHeight / 2), new Vector3((float)mapWidth, 0.0f, (float)mapHeight));
    }
}
