using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    int row, column, roomID;

    public void SetData(int _col, int _row, int _roomID)
    {
        row = _row;
        column = _col;
        roomID = _roomID;
    }
}
