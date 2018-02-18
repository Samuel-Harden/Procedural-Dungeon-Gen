using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePathGen : MonoBehaviour
{
    TileGen tileGen;


    private void Awake()
    {
        tileGen = GetComponent<TileGen>();
    }


    // A* Pathfinding
    public void FindPath(Vector3 _startPos, Vector3 _targetPos)
    {
        Tile startTile  = tileGen.GetTileAtWorldPos(_startPos);

        Tile targetTile = tileGen.GetTileAtWorldPos(_targetPos);

        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(startTile);

        while (openSet.Count > 0)
        {
            Tile currentTile = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].GetFCost() < currentTile.GetFCost() ||
                    openSet[i].GetFCost() == currentTile.GetFCost() &&
                    openSet[i].GetHCost() < currentTile.GetHCost())
                {
                    currentTile = openSet[i];
                }
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            if(currentTile == targetTile)
            {
                RetracePath(startTile, targetTile);
                return;
            }

            foreach (Tile neighbour in tileGen.GetNeighbours(currentTile))
            {
                if (!neighbour.GetWalkable() || closedSet.Contains(neighbour))
                        continue;

                int newMovementCostToNeighbour = currentTile.GetGCost() +
                    GetDistance(currentTile, neighbour);

                if (newMovementCostToNeighbour < neighbour.GetGCost() || !openSet.Contains(neighbour))
                {
                    neighbour.SetGCost(newMovementCostToNeighbour);
                    neighbour.SetHCost(GetDistance(neighbour, targetTile));
                    neighbour.parent = currentTile;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }


    // Now we have the route, retrace our steps
    private void RetracePath(Tile _startTile, Tile _endTile)
    {
        List<Tile> path = new List<Tile>();

        Tile currentTile = _endTile;

        while(currentTile != _startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }

        path.Reverse();

        tileGen.SetCorridor(path);
    }


    private int GetDistance(Tile _tileA, Tile _tileB)
    {
        int distX = Mathf.Abs(_tileA.GetRow() - _tileB.GetRow());
        int distZ = Mathf.Abs(_tileA.GetCol() - _tileB.GetCol());

        if(distX > distZ)
        {
            return distZ + 10 * (distX-distZ);
        }

        return distX + 10 * (distZ - distX);
    }
}
