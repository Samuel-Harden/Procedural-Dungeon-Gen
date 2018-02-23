using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGen : MonoBehaviour
{
    [SerializeField] GameObject tileContainer;

    [SerializeField] GameObject tilePrefab;
    [SerializeField] List<Sprite> tileSprites;
    [SerializeField] Camera camera;

    private TilePathGen tilePathGen;

    private Tile[,] tileMap;

    private List<int> spritePosValue;

    private int minPosX;
    private int maxPosX;
    private int minPosZ;
    private int maxPosZ;

    private int tileSize = 1;

    private int mapWidth;
    private int mapHeight;

    private bool enableDoubleConnection;

    Dictionary<int, int> sprites = new Dictionary<int, int>()
    {
        {   2,  1 }, {   8,  2 }, {  10,  3 }, {  11,  4 }, {  16,  5 },
        {  18,  6 }, {  22,  7 }, {  24,  8 }, {  26,  9 }, {  27,  10 },
        {  30, 11 }, {  31, 12 }, {  64, 13 }, {  66, 14 }, {  72, 15 },
        {  74, 16 }, {  75, 17 }, {  80, 18 }, {  82, 19 }, {  86, 20 },
        {  88, 21 }, {  90, 22 }, {  91, 23 }, {  94, 24 }, {  95, 25 },
        { 104, 26 }, { 106, 27 }, { 107, 28 }, { 120, 29 }, { 122, 30 },
        { 123, 31 }, { 126, 32 }, { 127, 33 }, { 208, 34 }, { 210, 35 },
        { 214, 36 }, { 216, 37 }, { 218, 38 }, { 219, 39 }, { 222, 40 },
        { 223, 41 }, { 248, 42 }, { 250, 43 }, { 251, 44 }, { 254, 45 },
        { 255, 46 }, {   0, 47 }
    };

    public void Initialize(List<Room> _rooms, Vector3 _dungeonCentre, bool _connection)
    {
        tilePathGen = GetComponent<TilePathGen>();

        minPosX = (int)_dungeonCentre.x;
        maxPosX = (int)_dungeonCentre.x;

        minPosZ = (int)_dungeonCentre.z;
        maxPosZ = (int)_dungeonCentre.z;

        enableDoubleConnection = _connection;

        GetDungeonBounds(_rooms);

        GenerateTileMap(_rooms);

        //CleanUpRooms(_rooms);
        camera.transform.position = new Vector3(mapWidth / 2, mapHeight, mapHeight / 2);
    }


    private void GenerateTileMap(List<Room> _rooms)
    {
        mapWidth = maxPosX - minPosX / tileSize;
        mapHeight = maxPosZ - minPosZ / tileSize;

        mapWidth += 2;
        mapHeight += 2;

        tileMap = new Tile[mapHeight, mapWidth];

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
                    if (room.GenerateTile())
                    {

                        tileMap[posZ - minPosZ, posX - minPosX] =
                            CreateTile(spawnPosX, spawnPosZ, room.GetRoomID(), true);

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
                if (tileMap[h, w] == null)
                {
                    tileMap[h, w] = CreateTile(w, h, -1, false);
                }
            }
        }

        GenerateCorridorData(_rooms);

        foreach (Tile tile in tileMap)
        {
            AssignSprite(tile);

            AssignInaccessible(tile);
        }

        //Debug.Log(tileMap.Length);
    }


    public void LoadTileMap(Tile[,] _tilemap)
    {

    }


    public void UpdateTiles()
    {
        foreach(Tile tile in tileMap)
        {
            // If a tile needs updating
            if(tile.Update())
                AssignSprite(tile);
        }
    }


    private Tile CreateTile(int _posX, int _posZ, int _roomID, bool _walkable)
    {

        var tile = Instantiate(tilePrefab, new Vector3(_posX, 0, _posZ),
            tilePrefab.transform.rotation);

        tile.GetComponent<Tile>().SetData(_posX, _posZ, _roomID, _walkable);

        tile.GetComponent<TileSelect>().SetReferences(this.gameObject);

        if (_roomID == -1)
            tile.GetComponent<Renderer>().material.color = Color.black;

        tile.transform.SetParent(tileContainer.transform);

        return tile.GetComponent<Tile>();
    }


    private void GenerateCorridorData(List<Room> _rooms)
    {
        foreach (Room room in _rooms)
        {
            for (int i = room.GetConnectedRooms().Count - 1; i >= 0; i--)
            {
                tilePathGen.FindPath(room.transform.position,
                    room.GetConnectedRoom(i).transform.position);

                if (!enableDoubleConnection)
                    room.GetConnectedRoom(i).RemoveDuplicateConnection(room.transform.position);
            }
        }
    }


    // sets a tile to be of type Corridor (Currently only changing colour but will change data in later update)
    public void SetCorridor(List<Tile> _path)
    {
        foreach (Tile tile in _path)
        {
            // if this tile is not a room or corridor
            if (tile.GetRoomID() == -1)
            {
                tile.SetRoomID(0);
                tile.GetComponent<Renderer>().material.color = Color.white;
            }
        }
    }


    public void AssignSprite(Tile _tile)
    {
        int tileType = 0;
        int index    = 0;

        bool up    = false;
        bool down  = false;
        bool left  = false;
        bool right = false;


        // If room is not blank (ie a room or corridor)
        if(_tile.GetRoomID() != -1)
        {
            //Check Up
            if(tileMap[_tile.GetRow() + 1, _tile.GetCol()].GetRoomID() != -1)
            {
                up = true;
                index += 2;
                tileMap[_tile.GetRow() + 1, _tile.GetCol()].SetUpdate(true);
            }

            //Check Down
            if (tileMap[_tile.GetRow() - 1, _tile.GetCol()].GetRoomID() != -1)
            {
               down = true;
               index += 64;
                tileMap[_tile.GetRow() - 1, _tile.GetCol()].SetUpdate(true);
            }

            //Check Left
            if (tileMap[_tile.GetRow(), _tile.GetCol() - 1].GetRoomID() != -1)
            {
                left = true;
                index += 8;
                tileMap[_tile.GetRow(), _tile.GetCol() - 1].SetUpdate(true);
            }

            //Check Right
            if (tileMap[_tile.GetRow(), _tile.GetCol() + 1].GetRoomID() != -1)
            {
                right = true;
                index += 16;
                tileMap[_tile.GetRow(), _tile.GetCol() + 1].SetUpdate(true);
            }

            // Check Up and Left
            if(up && left)
            {
                if (tileMap[_tile.GetRow() + 1, _tile.GetCol() - 1].GetRoomID() != -1)
                {
                    index += 1;
                    tileMap[_tile.GetRow() + 1, _tile.GetCol() - 1].SetUpdate(true);
                }
            }

            // Check Up and Right
            if (up && right)
            {
                if (tileMap[_tile.GetRow() + 1, _tile.GetCol() + 1].GetRoomID() != -1)
                {
                    index += 4;
                    tileMap[_tile.GetRow() + 1, _tile.GetCol() + 1].SetUpdate(true);
                }
            }

            // Check Down and Left
            if (down && left)
            {
                if (tileMap[_tile.GetRow() - 1, _tile.GetCol() - 1].GetRoomID() != -1)
                {
                    index += 32;
                    tileMap[_tile.GetRow() - 1, _tile.GetCol() - 1].SetUpdate(true);
                }
            }

            // Check Down and Right
            if (down && right)
            {
                if (tileMap[_tile.GetRow() - 1, _tile.GetCol() + 1].GetRoomID() != -1)
                {
                    index += 128;
                    tileMap[_tile.GetRow() - 1, _tile.GetCol() + 1].SetUpdate(true);
                }
            }

            //tile.SetSpriteIndex(index);

            _tile.SetSpriteIndex(index);

            _tile.GetComponent<SpriteRenderer>().sprite = tileSprites[GetLookUpValue(index)];
        }
    }


    private void AssignInaccessible(Tile tile)
    {
        if (tile.GetCol() == 0 || tile.GetCol() == (mapWidth - 1) ||
            tile.GetRow() == 0 || tile.GetRow() == (mapHeight - 1))
            tile.GetComponent<TileSelect>().SetInaccessible();
    }


    private int GetLookUpValue(int _value)
    {
        int value = 0;
        sprites.TryGetValue(_value, out value);
        return value;
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
        int x = Mathf.RoundToInt(_worldPos.x) - minPosX;
        int z = Mathf.RoundToInt(_worldPos.z) - minPosZ;

        return tileMap[z, x];
    }


    // Returns a list of neighbours tiles
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


    public Tile[,] GetTileMap()
    {
        return tileMap;
    }


    public int GetMapWidth()
    {
        return mapWidth;
    }


    public int GetMapHeight()
    {
        return mapHeight;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        /*Gizmos.DrawWireCube(new Vector3(((float)maxPosX + (float)minPosX) / 2, 0.0f,
            ((float)maxPosZ + (float)minPosZ) / 2),
            new Vector3((float)maxPosX - (float)minPosX, 0.0f,
            (float)maxPosZ - (float)minPosZ));*/

        //Gizmos.DrawWireCube(new Vector3((float)mapWidth / 2, 0.0f,
        //(float)mapHeight / 2), new Vector3((float)mapWidth, 0.0f, (float)mapHeight));

        /*if (path != null)
        {
            foreach(Tile tile in tileMap)
            {
                if (path.Contains(tile))
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(tile.transform.position, Vector3.one * (1 - 0.1f));
                }
            }
        }*/
    }
}
