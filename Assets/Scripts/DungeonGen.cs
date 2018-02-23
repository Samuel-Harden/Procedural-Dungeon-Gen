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
    [SerializeField]
    int smallRoomPercentage;

    [SerializeField] Transform roomContainer;
    [SerializeField] Camera camera;

    private int minDungeonRadius = 50;
    private int maxDungeonRadius = 100;

    private RoomGen roomGen;
    private TileGen tileGen;
    private PathGen pathGen;
    private MouseStatus mouseStatus;

    private List<Room> rooms;

    private bool setupComplete;

    private bool generateLevel = false;
    private bool firstMap = true;


    private void Start()
    {
        roomGen     = GetComponent<RoomGen>();
        tileGen     = GetComponent<TileGen>();
        pathGen     = GetComponent<PathGen>();
        mouseStatus = GetComponent<MouseStatus>();

        rooms = new List<Room>();
    }


    private void Update()
    {
        if (generateLevel)
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
                    foreach (Room room in rooms)
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

                        if (generateTiles)
                            tileGen.Initialize(pathGen.GetConnectedRooms(), dungeonCentre,
                                EnableDoubleConnections);

                        camera.transform.position = new Vector3(tileGen.GetMapWidth() / 2,
                            tileGen.GetMapHeight(), tileGen.GetMapHeight() / 2);

                        int minZoom = 0;

                        if (tileGen.GetMapHeight() > tileGen.GetMapWidth())
                            minZoom = tileGen.GetMapHeight();

                        else
                            minZoom = tileGen.GetMapWidth();

                        mouseStatus.SetZoomLevel(minZoom);

                        setupComplete = true;
                    }
                }
            }
        }

        // Once setup is complete we can just update our tiles
        if (setupComplete)
        {
            tileGen.UpdateTiles();
            generateLevel = false;
        }
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


    public void GenerateLevel()
    {
        ResetMap();

        CheckSetDungeonSize();

        roomGen.GenerateRooms(rooms, roomCount,
            dungeonRadius, randNoRooms);

        LimitSmallRoomCount();

        generateLevel = true;
    }


    private void ResetMap()
    {
        foreach(Room room in rooms)
        {
            if(room != null)
                Destroy(room.gameObject, 0.2f);
        }

        rooms.Clear();

        tileGen.ClearTiles();

        pathGen.ClearRooms();

        roomGen.ResetRoomCount();

        setupComplete = false;

        firstMap = false;
    }
}
