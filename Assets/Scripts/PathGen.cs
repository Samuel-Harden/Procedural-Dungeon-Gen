using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathGen : MonoBehaviour
{
    int roomNo = 0;

    List<Room> roomsToCheck;
    List<Room> connectedRooms;
    List<Room> unConnectedRooms;

    int smallRoomPercentage;


    public void Initialize(List<Room> _rooms, int _smallRoomPercentage)
    {
        smallRoomPercentage = _smallRoomPercentage;

        roomsToCheck = new List<Room>();

        DiscardSmallRooms(_rooms);

        ConnectRooms();
    }


    private void DiscardSmallRooms(List<Room> _rooms)
    {
        foreach (Room room in _rooms)
        {
            // Random Chance to add a small room
            if(Random.Range(0, 100) < smallRoomPercentage)
            {
                roomsToCheck.Add(room);
                continue;
            }

            // if room is med or large and room is not the main room
            if (room.GetRoomType() != 0)
                roomsToCheck.Add(room);

            else
                //room.gameObject.SetActive(false);
                Destroy(room.gameObject);
        }

    }


    private void ConnectRooms()
    {
        connectedRooms   = new List<Room>();
        unConnectedRooms = new List<Room>();

        // dont need as it gets added in loop below...
        //connectedRooms.Add(roomsToCheck[0]);

        // Set First room as main essentially
        roomsToCheck[0].ConnectToMain();

        // Connect any rooms that share borders,
        // Will also update connected to Main if main room
        // Shares a border(s)
        ConnectBorderingRooms();

        // Create lists for connected and unconnected rooms
        foreach (Room room in roomsToCheck)
        {
            if (!room.ConnectedToMain())
                unConnectedRooms.Add(room);

            else
                connectedRooms.Add(room);
        }

        // pass in first room
        ConnectNextRoom(connectedRooms[0]);

        // Add rooms ready for tile generation
        foreach (Room room in connectedRooms)
        {
            room.AddToTileGeneration();
        }

        // Clear out any duplicates
        ClearDuplicates();
    }


    private void ConnectBorderingRooms()
    {
        foreach (Room room in roomsToCheck)
        {
            for (int i = 0; i < roomsToCheck.Count; i++)
            {
                if (room.GetRoomID() != roomsToCheck[i].GetRoomID())
                {
                    room.BorderCheck(roomsToCheck[i]);
                }
            }
        }

        // Clears any rooms that have connected to a room twice
        ClearDuplicates();

        roomsToCheck[0].CheckConnectionToMain(roomsToCheck[0].GetRoomID());
    }


    private void ConnectNextRoom(Room _room)
    { 
        // sort by how close they are to the next conected room 
        unConnectedRooms = unConnectedRooms.OrderBy(room => Vector3.Distance(room.transform.position, _room.transform.position)).ToList();

        float dist = Vector3.Distance(_room.transform.position, unConnectedRooms[0].transform.position);
        
        for (int i = 0; i < connectedRooms.Count; i++)
        {
            if (Vector3.Distance(connectedRooms[i].transform.position, unConnectedRooms[0].transform.position) < dist)
            {
                dist = Vector3.Distance(unConnectedRooms[0].transform.position, connectedRooms[i].transform.position);
                roomNo = i;
            }
        }

        // Now have closest room, Connect Rooms to each other
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


    private void ClearDuplicates()
    {
        Debug.Log("Cheese");
        foreach (Room room in connectedRooms)
        {
            room.TidyConnected();
        }
    }


    public List<Room> GetConnectedRooms()
    {
        return connectedRooms;
    }
}
