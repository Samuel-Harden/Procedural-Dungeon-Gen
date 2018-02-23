using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelect : MonoBehaviour
{
    private TileGen tileGen;
    private MouseStatus mouse;
    private Tile tile;

    private bool inaccessible;
    private Color color;


    private void Start()
    {
        tile = GetComponent<Tile>();
        color = GetComponent<SpriteRenderer>().material.color;
    }


    private void OnMouseOver()
    {
        GetComponent<SpriteRenderer>().material.color = Color.green;

        if (mouse.mouseLeftDown)
        {
            if (inaccessible)
                return;

            tile.SetWalkable(true);

            tile.SetRoomID(0);
            color = Color.white;
            tile.SetUpdate(true);

            return;
        }

        if (mouse.mouseRightDown)
        {
            if (inaccessible)
                return;

            tile.SetWalkable(false);

            tile.SetSpriteIndex(0);

            tile.SetRoomID(-1);

            color = Color.black;

            tile.ResetSprite();

            return;
        }
    }


    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().material.color = color;
    }


    public void SetReferences(GameObject _dungeonGen)
    {
        tileGen = _dungeonGen.GetComponent<TileGen>();
        mouse   = _dungeonGen.GetComponent<MouseStatus>();
    }


    public void SetInaccessible()
    {
        inaccessible = true;
    }
}
