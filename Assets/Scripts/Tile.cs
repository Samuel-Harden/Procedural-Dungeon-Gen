using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int row;
    private int column;
    private int roomID;
    private int roomValue;


    public void SetData(int _col, int _row, int _roomID)
    {
        row       = _row;
        column    = _col;
        roomID    = _roomID;
        roomValue = row + column;
    }


    // Position Value of tile on map
    public int GetRoomValue()
    {
        return roomValue;
    }


    // Tile is part of room...
    public int GetRoomID()
    {
        return roomID;
    }
}
