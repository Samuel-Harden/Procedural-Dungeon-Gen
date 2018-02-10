using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathGen : MonoBehaviour
{
    int mainRoomID;

    int roomNo = 0;

    List<Room> roomsToCheck;

    List<Room> connectedRooms;
    List<Room> unConnectedRooms;

    public void Initialize(List<Room> _rooms)
    {
        roomsToCheck = new List<Room>();

        ClearSmallRooms(_rooms);

        //IdentifyMainRoom(_rooms);

        ConnectRooms();

        //abc();
    }


    private void IdentifyMainRoom(List<Room>_rooms)
    {
        int roomSize = 0;
        mainRoomID = 0;

        foreach(Room room in roomsToCheck)
        {
            if (room.GetRoomArea() > roomSize)
            {
                mainRoomID = room.GetRoomID();
                // Connect room to main

            }
        }
    }


    private void ClearSmallRooms(List<Room> _rooms)
    {
        foreach (Room room in _rooms)
        {
            // if room is med or large and room is not the main room
            if (room.GetRoomType() != 0)
                roomsToCheck.Add(room);
        }

    }


    private void ConnectBorderingRooms()
    {
        foreach(Room room in roomsToCheck)
        {
            for (int i = 0; i < roomsToCheck.Count; i++)
            {
                if(room.GetRoomID() != roomsToCheck[i].GetRoomID())
                {
                    room.BorderCheck(roomsToCheck[i]);
                }
            }
        }
    }


    private void ConnectRooms()
    {
        connectedRooms   = new List<Room>();
        unConnectedRooms = new List<Room>();

        connectedRooms.Add(roomsToCheck[0]);

        // Set First room as main essentially
        connectedRooms[0].ConnectToMain();

        // Create list for all unconnected rooms
        foreach (Room room in roomsToCheck)
        {
            if (!room.ConnectedToMain())
                unConnectedRooms.Add(room);
        }

        //Debug.Log(unConnectedRooms.Count);

        ConnectBorderingRooms();

        // pass in first room
        ConnectNextRoom(connectedRooms[0]);
    }


    private void ConnectNextRoom(Room _room)
    { 
         unConnectedRooms = unConnectedRooms.OrderBy(room => Vector3.Distance(room.transform.position, _room.transform.position)).ToList();

        float dist = 1000.0f;
        
        for (int i = 0; i < connectedRooms.Count; i++)
        {
            if (Vector3.Distance(unConnectedRooms[0].transform.position, connectedRooms[i].transform.position) < dist)
            {
                dist = Vector3.Distance(unConnectedRooms[0].transform.position, connectedRooms[i].transform.position);
                roomNo = i;
            }
        }

        // Now have closest room, Connect Rooms
        connectedRooms[roomNo].AddConnectedRoom(unConnectedRooms[0]);
        unConnectedRooms[0].AddConnectedRoom(connectedRooms[roomNo]);

        // Add to connected Rooms list
        unConnectedRooms[0].ConnectToMain();
        connectedRooms.Add(unConnectedRooms[0]);


        // Remove from unconnected list
        unConnectedRooms.RemoveAt(0);

        roomNo++;

        if (unConnectedRooms.Count > 0)
            ConnectNextRoom(connectedRooms[roomNo]);
    }
}
