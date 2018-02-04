using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{
    [Header("No of Rooms")]
    [SerializeField] int roomCount = 0;
    [SerializeField] int dungeonRadius = 0;

    [SerializeField] bool randNoRooms;
    [SerializeField] bool randDungeonSize;
    private int minDungeonRadius = 50;
    private int maxDungeonRadius = 100;

    private RoomGen roomGen;
    private List<Vector3> positions;
    private List<Room> rooms;

    private void Start()
    {
        roomGen = gameObject.GetComponent<RoomGen>();

        positions = new List<Vector3>();
        rooms = new List<Room>();

        CheckSetDungeonSize();

        roomGen.GenerateRooms(rooms, roomCount, dungeonRadius, randNoRooms);
    }


    private void FixedUpdate()
    {
        foreach (Room room in rooms)
        {
            room.CheckSpacing(rooms);
        }

        //rooms[0].CheckSpacing(rooms);
    }

    private void CheckSetDungeonSize()
    {
        if(!randDungeonSize)
        {
            if (dungeonRadius >= minDungeonRadius && dungeonRadius <= maxDungeonRadius)
                return;
        }

        //dungeonRadius = Random.Range(minDungeonRadius, maxDungeonRadius);
    }


    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        foreach (Room room in rooms)
        {
            Gizmos.DrawWireSphere(room.GetRoomPos(), 1);
        }
    }*/
}
