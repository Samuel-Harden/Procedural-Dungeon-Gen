using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{
    [SerializeField] int roomCount = 0;
    [SerializeField] int dungeonRadius = 0;

    [SerializeField] bool randNoRooms;
    [SerializeField] bool randDungeonSize;
    [SerializeField] bool generateTiles;
    [SerializeField] bool EnableDoubleConnections;

    [Header("% of Small Rooms (Cannot exceed 100%)")]
    [SerializeField] int smallRoomPercentage;

    [SerializeField] Transform roomContainer;

    private int minDungeonRadius = 50;
    private int maxDungeonRadius = 100;

    private RoomGen roomGen;

    private List<Vector3> positions;
    private List<Room> rooms;

    private bool setupComplete;

    private TileGen tileGenerator;
    private PathGen pathGen;


    private void Start()
    {
        roomGen       = gameObject.GetComponent<RoomGen>();
        tileGenerator = gameObject.GetComponent<TileGen>();
        pathGen       = gameObject.GetComponent<PathGen>();

        positions = new List<Vector3>();
        rooms     = new List<Room>();

        CheckSetDungeonSize();

        roomGen.GenerateRooms(rooms, roomCount,
            dungeonRadius, randNoRooms);

        LimitSmallRoomCount();
    }


    private void Update()
    {
        bool overlap = false;

        if (!setupComplete)
        {
            foreach (Room room in rooms)
            {
                room.CheckSpacing(rooms);
            }

            foreach (Room room in rooms)
            {
                if (room.IsOverlapping())
                    overlap = true;
            }

            if (!overlap)
            {
                foreach(Room room in rooms)
                {
                    room.SetPos();
                }

                foreach (Room room in rooms)
                {
                    if (room.IsOverlapping())
                        overlap = true;
                }

                if (!overlap)
                {
                    Vector3 dungeonCentre = new Vector3((float)dungeonRadius / 2, 0.0f,
                        (float)dungeonRadius / 2);

                    AddRoomsToContainer();

                    pathGen.Initialize(rooms, smallRoomPercentage);

                    if(generateTiles)
                        tileGenerator.Initialize(pathGen.GetConnectedRooms(), dungeonCentre,
                            EnableDoubleConnections);

                    setupComplete = true;
                }
            }
        }

        // Once setup is complete we can just update our tiles
        if (setupComplete)
            tileGenerator.UpdateTiles();
    }


    private void AddRoomsToContainer()
    {
        foreach (Room room in rooms)
        {
            room.transform.SetParent(roomContainer);
        }
    }


    private void LimitSmallRoomCount()
    {
        if (smallRoomPercentage > 100)
            smallRoomPercentage = 100;

        if (smallRoomPercentage < 0)
            smallRoomPercentage = 0;
    }


    private void CheckSetDungeonSize()
    {
        // Must be at least 30 or bugs occur with room sizes!
        if (dungeonRadius < 30)
            dungeonRadius = 30;

        if (!randDungeonSize)
        {
            if (dungeonRadius >= minDungeonRadius && dungeonRadius <= maxDungeonRadius)
                return;
        }

        //dungeonRadius = Random.Range(minDungeonRadius, maxDungeonRadius);
    }


    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        foreach (Tile tile in grid)
        {
            Gizmos.DrawWireSphere(tile.transform.position, 1);
        }
    }*/
}
