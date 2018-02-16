﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Room : MonoBehaviour
{
    private int roomWidth;
    private int roomHeight;

    private Vector3 roomBoundsPoint;

    Vector3 velocity;
    Vector3 acceleration;

    private float maxSpeed = 0.5f;
    private float maxForce = 0.5f;
    private bool overlapping;
    private bool connectedToMain;
    private bool connectionCheckComplete;
    private bool generateTile;

    private int roomID;
    private int roomType;

    private List<Room> connectedRooms;

    public void Initialise(int _roomWidth, int _roomHeight, Vector3 _roomPos, int _roomType, int _roomID)
    {
        roomWidth  = _roomWidth;
        roomHeight = _roomHeight;
        roomID     = _roomID;
        roomType = _roomType;

        overlapping = true;
        connectedToMain = false;
        connectionCheckComplete = false;

        connectedRooms = new List<Room>();
    }


    public void CheckSpacing(List<Room> _rooms)
    {
        roomBoundsPoint = new Vector3(transform.position.x - roomWidth / 2, 0.0f,
            transform.position.z - roomHeight / 2);

        List<Room> overlappingRooms = new List<Room>();

        overlappingRooms = OverlapCheck(_rooms);

        if (overlappingRooms.Count != 0)
        {
            Vector3 sep = Seperate(overlappingRooms);

            acceleration += sep;

            velocity += acceleration;

            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

            transform.position = (transform.position + velocity);

            // reset acceleration to 0 each cycle
            acceleration = Vector3.zero;

            velocity = Vector3.zero;

            overlapping = true;

            return;
        }

        overlapping = false;
    }


    List<Room> OverlapCheck(List<Room> _rooms)
    {
        List<Room> overlappingRooms = new List<Room>();

        for (int i = 0; i < _rooms.Count; i++)
        {
            if (i == roomID)
                continue;
            // ((A.X + A.Width) > (B.X) &&
            if ((roomBoundsPoint.x + roomWidth)
                > (_rooms[i].GetRoomBoundsPoint().x) &&
                // (A.X) < (B.X + B.Width) &&
                (roomBoundsPoint.x) < (_rooms[i].GetRoomBoundsPoint().x
                    + _rooms[i].GetWidth()) &&

                // (A.Y + A.Height) > (B.Y) &&
                (roomBoundsPoint.z + roomHeight)
                > (_rooms[i].GetRoomBoundsPoint().z) &&
                // (A.Y) < (B.Y + B.Height))
                (roomBoundsPoint.z) < (_rooms[i].GetRoomBoundsPoint().z
                    + _rooms[i].GetHeight()))
            {
                overlappingRooms.Add(_rooms[i]);
            }
        }
        return overlappingRooms;
    }


    public void BorderCheck(Room _room)
    {
        // If im already connected to this room, return
        foreach(Room room in connectedRooms)
        {
            if(_room.GetRoomID() == room.GetRoomID())
            {
                return;
            }
        }

        // ((A.X + A.Width) >= (B.X) &&
        if ((roomBoundsPoint.x + roomWidth)
                >= (_room.GetRoomBoundsPoint().x) &&
            // (A.X) <= (B.X + B.Width) &&
            (roomBoundsPoint.x) <= (_room.GetRoomBoundsPoint().x
                + _room.GetWidth()) &&

            // (A.Y + A.Height) >= (B.Y) &&
            (roomBoundsPoint.z + roomHeight)
                >= (_room.GetRoomBoundsPoint().z) &&
            // (A.Y) <= (B.Y + B.Height))
            (roomBoundsPoint.z) <= (_room.GetRoomBoundsPoint().z
                + _room.GetHeight()))
        {
            // Connect Rooms  // prob why I have to clear Duplicates!!!
            connectedRooms.Add(_room);
            _room.connectedRooms.Add(this);
        }
    }


    public void CheckConnectionToMain(int _ignorePrev)
    {
        if(!connectionCheckComplete)
        {
            connectionCheckComplete = true;
            connectedToMain = true;

            // Connect to main, call other connected room
            // Excluding previous
            foreach (Room room in connectedRooms)
            {
                // Ignore previous
                if(room.GetRoomID() != _ignorePrev)
                {
                    room.ConnectedToMain();
                    room.CheckConnectionToMain(room.GetRoomID());
                }
            }
        }
    }


    Vector3 Seperate(List<Room> _rooms)
    {
        Vector3 steer = Vector3.zero;
        int count = 0;

        // check through every other _room
        for (int i = 0; i < _rooms.Count; i++)
        {
            float d = Vector3.Distance(transform.position, _rooms[i].transform.position);
            // Calculate vector pointing away from other rooms
            if (d > 0)
            {
                Vector3 diff = (transform.position - _rooms[i].transform.position);
                diff.Normalize();
                diff = (diff / d); // Weight by distance
                steer = (steer + diff);
                count++;
            }
        }

        // Average -- divided by how many
        if (count > 0)
        {
            steer = (steer / count);
        }

        // as long as the vector is greater than 0
        if (steer != Vector3.zero)  // if(steer.mag() > 0)
        {
            //steer.setMag (maxSpeed);
            steer = Vector3.ClampMagnitude(steer, maxSpeed);

            steer.Normalize();
            steer = (steer * maxSpeed);
            steer = (steer - velocity);
        }

        return steer;
    }


    public void TidyConnected()
    {
        connectedRooms = connectedRooms.Distinct().ToList();
    }


    public int GetWidth()
    {
        return roomWidth;
    }


    public int GetHeight()
    {
        return roomHeight;
    }


    // This is the Bottom Left Pos of the room
    // pos - (half room width and height)
    // needed for bounds check...
    public Vector3 GetRoomBoundsPoint()
    {
        return roomBoundsPoint;
    }


    public bool IsOverlapping()
    {
        return overlapping;
    }


    public int GetRoomID()
    {
        return roomID;
    }


    public int GetRoomType()
    {
        return roomType;
    }


    public int GetRoomArea()
    {
        return roomWidth * roomHeight;
    }


    public void ConnectToMain()
    {
        connectedToMain = true;
    }


    public bool ConnectedToMain()
    {
        return connectedToMain;
    }


    public void AddConnectedRoom(Room connections)
    {
        connectedRooms.Add(connections);
    }


    public List<Room> GetConnectedRooms()
    {
        return connectedRooms;
    }


    public Room GetConnectedRoom(int _index)
    {
        return connectedRooms[_index];
    }


    public void RemoveDuplicateConnection(Vector3 _roomPos)
    {
        foreach(Room room in connectedRooms)
        {
            if(_roomPos == room.transform.position)
            {
                connectedRooms.Remove(room);
                return;
            }
        }
    }


    public void SetPos()
    {
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x),
        0.0f, Mathf.RoundToInt(transform.position.z));

        roomBoundsPoint = new Vector3(Mathf.RoundToInt(transform.position.x - roomWidth / 2),
        0.0f, Mathf.RoundToInt(transform.position.z - roomHeight / 2));
    }


    // For small Rooms, should this one be kept
    // for final dungeon build?
    public void AddToTileGeneration()
    {
        generateTile = true;
    }


    public bool GenerateTile()
    {
        return generateTile;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if(roomType != 0)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }

        if (roomType == 0)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }

        if (connectedRooms != null)
        {
            foreach (Room room in connectedRooms)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, room.transform.position);
            }
        }
    }
}
