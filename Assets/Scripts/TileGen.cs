using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGen : MonoBehaviour
{
    [SerializeField] GameObject tileContainer;

    [SerializeField] GameObject tilePrefab;

    private Tile[,] tileMap;

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

        tileMap = new Tile[mapHeight, mapWidth];

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
                            CreateTile(spawnPosX, spawnPosZ, room.GetRoomID(), roomCol, true);

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

                    tileMap[h, w] = CreateTile(w, h, -1, roomCol, true);
                }
            }
        }

        Debug.Log(tileMap.Length);
    }


    private Tile CreateTile(int _posX, int _posZ, int _roomID, Color _roomCol, bool _walkable)
    {
        Color roomCol = _roomCol;

        var tile = Instantiate(tilePrefab, new Vector3(_posX, 0, _posZ),
            Quaternion.identity);

        tile.GetComponent<Tile>().SetData(_posX, _posZ, _roomID, _walkable);

        tile.GetComponent<Renderer>().material.color = roomCol;

        tile.transform.SetParent(tileContainer.transform);

        return tile.GetComponent<Tile>();
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


    public Tile GetTileAtWorldPos(Vector3 _worldPos)
    {
        int x = Mathf.RoundToInt(_worldPos.x);
        int z = Mathf.RoundToInt(_worldPos.z);

        return tileMap[z, x];
    }


    public List<Tile> GetNeighbours(Tile _tile)
    {
        List<Tile> neighbours = new List<Tile>();

        // Left
        if (_tile.GetCol() - 1 >= 0)
        {
            // Add tile to the left
            neighbours.Add(tileMap[_tile.GetRow(), _tile.GetCol() - 1]);
        }

        // Right
        if (_tile.GetCol() + 1 < mapWidth)
        {
            // Add tile to the right
            neighbours.Add(tileMap[_tile.GetRow(), _tile.GetCol() + 1]);
        }

        // Down
        if (_tile.GetRow() - 1 >= 0)
        {
            // Add tile below
            neighbours.Add(tileMap[_tile.GetRow() - 1, _tile.GetCol()]);
        }

        // Up
        if (_tile.GetRow() + 1 < mapHeight)
        {
            // Add tile above
            neighbours.Add(tileMap[_tile.GetRow() + 1, _tile.GetCol()]);
        }

        return neighbours;
    }


    public List<Tile> path;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        /*Gizmos.DrawWireCube(new Vector3(((float)maxPosX + (float)minPosX) / 2, 0.0f,
            ((float)maxPosZ + (float)minPosZ) / 2),
            new Vector3((float)maxPosX - (float)minPosX, 0.0f,
            (float)maxPosZ - (float)minPosZ));*/

        //Gizmos.DrawWireCube(new Vector3((float)mapWidth / 2, 0.0f,
        //(float)mapHeight / 2), new Vector3((float)mapWidth, 0.0f, (float)mapHeight));

        if (path.Count != 0)
        {
            foreach(Tile tile in tileMap)
            {
                if (path.Contains(tile))
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(tile.transform.position, Vector3.one * (1 - 0.1f));
                }
            }
        }
    }
}
