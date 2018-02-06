using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector3 world_pos;

    public Tile(Vector3 _world_pos)
    {
        
        world_pos = _world_pos;
    }
}
