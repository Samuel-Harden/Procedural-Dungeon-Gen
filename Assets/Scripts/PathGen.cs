using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathGen : MonoBehaviour
{
    int mainRoomID;

    Vector3 mainRoomPos;

    // identlfy main
    // connect 2-3 nearest rooms

    List<Room> roomsToCheck;

    Vector3 pos1;
    Vector3 pos2;
    Vector3 Po3;

    Vector3 dir1;
    Vector3 dir2;


    private void Start()
    {
        abc();
    }


    public void Initialize(List<Room> _rooms)
    {
        roomsToCheck = new List<Room>();

        ClearSmallRooms(_rooms);

        IdentifyMainRoom();

        //ConnectRooms();

        abc();
    }


    private void IdentifyMainRoom()
    {
        int roomSize = 0;
        mainRoomID = 0;

        foreach(Room room in roomsToCheck)
        {
            if (room.GetRoomArea() > roomSize)
            {
                mainRoomID = room.GetRoomID();
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


    private void abc()
    {
        // order positions by X axis
        // Sweep across
        // start with the first three points, find the circumcentre and Radius
        // check if anyother points are in its area, valid triangle if so



        List<Vector3> positions = new List<Vector3>();

        roomsToCheck = roomsToCheck.OrderBy(c => c.transform.position.x).ToList();

        positions.Add(roomsToCheck[0].transform.position);
        positions.Add(roomsToCheck[1].transform.position);
        positions.Add(roomsToCheck[2].transform.position);

        //Check where 2 sides they cross

        // find 1st midpoint
        Vector3 pos1 = positions[0] + positions[1] / 2;

        // heading = target - pos
        Vector3 dir1 = positions[1] - positions[0] + Vector3.right;


        Vector3 pos2 = positions[1] + positions[2] / 2;

        Vector3 dir2 = positions[2] - positions[1] + Vector3.right;
    }


    private void ConnectRooms()
    {
        // Sweep across
        // start withthe first three points, find the circumcentre and Radius
        // check if anyother points are in its area, valid triangle if so

        /*for (int i = 0; i < roomsToCheck.Count; i++)
        {
            List<Room> nearestRooms = new List<Room>();

            if (!roomsToCheck[i].ConnectedToMain())
            {
                roomsToCheck[i].ConnectToMain();

                foreach (Room room in roomsToCheck)
                {
                    // if room is not this room
                    if (room.transform.position != roomsToCheck[i].transform.position /*&& !room.ConnectedToMain())
                        nearestRooms.Add(room);
                }

                //order by distance
                nearestRooms = nearestRooms.OrderBy(room => Vector3.Distance(room.transform.position,
                    roomsToCheck[i].transform.position)).ToList();

                if (nearestRooms.Count > 0)
                {
                    nearestRooms[0].SetConnectedRooms(roomsToCheck[i]);
                    nearestRooms[1].SetConnectedRooms(roomsToCheck[i]);
                    //Debug.DrawLine(roomsToCheck[i].transform.position, nearestRooms[0].transform.position);
                }
            }
        }*/
    }
}
