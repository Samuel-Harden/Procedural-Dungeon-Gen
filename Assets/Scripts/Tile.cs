﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int row;
    private int column;
    private int roomID;
    private int roomValue;

    public int gCost; // Cost to move to pos (Some tiles could cost more to move to for example)
    public int hCost; // How far node is away from target pos
    // F cost is both combined

    private bool walkable;

    public Tile parent;


    public void SetData(int _col, int _row, int _roomID, bool _walkable)
    {
        row       = _row;
        column    = _col;
        roomID    = _roomID;
        roomValue = row + column;

        walkable = _walkable;
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


    public int GetFCost()
    {
        return gCost + hCost;
    }


    public int GetRow()
    {
        return row;
    }


    public int GetCol()
    {
        return column;
    }


    public bool GetWalkable()
    {
        return walkable;
    }
}
