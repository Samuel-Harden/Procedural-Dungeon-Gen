using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{
    [SerializeField] int roomCount = 0;
    [SerializeField] int dungeonRadius = 0;

    [SerializeField] bool randNoRooms;
    [SerializeField] bool randDungeonSize;

    [SerializeField] Transform roomContainer;

    private int minDungeonRadius = 50;
    private int maxDungeonRadius = 100;

    private RoomGen roomGen;
    private List<Vector3> positions;
    private List<Room> rooms;

    private bool setupComplete;

    private TileGeneration tileGenerator;
    //private Triangulation triangulate;
    private PathGen pathGen;

    private List<Vector3> roomPos;
    private List<Vector3> roomConnections;

    private void Start()
    {
        roomGen = gameObject.GetComponent<RoomGen>();
        tileGenerator = gameObject.GetComponent<TileGeneration>();
        //triangulate = gameObject.GetComponent<Triangulation>();
        pathGen = gameObject.GetComponent<PathGen>();

        positions = new List<Vector3>();
        rooms = new List<Room>();

        roomPos = new List<Vector3>();
        roomConnections = new List<Vector3>();

        CheckSetDungeonSize();

        roomGen.GenerateRooms(rooms, roomCount, dungeonRadius, randNoRooms);
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
                //test.GenerateLayout(rooms);

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

                    pathGen.Initialize(rooms);

                    tileGenerator.Initialize(rooms, dungeonCentre);

                    setupComplete = true;
                }
            }
        }
    }


    private void AddRoomsToContainer()
    {
        foreach (Room room in rooms)
        {
            room.transform.SetParent(roomContainer);
        }
    }


    private void CheckSetDungeonSize()
    {
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
