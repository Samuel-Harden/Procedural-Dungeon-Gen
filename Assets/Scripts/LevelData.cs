using System;
using System.Collections.Generic;

[Serializable]
public class LevelData
{
    public List<int> tileTypes = new List<int>();
    public int mapWidth;
    public int mapHeight;
}