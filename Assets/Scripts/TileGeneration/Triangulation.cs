using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Triangulation : MonoBehaviour
{
    //private List<Room> orderedRooms;

    //List<List<Vector3>> triangles;

    /*public void Initialize(List<Room> _rooms)
    {
        orderedRooms = new List<Room>();

        OrderRooms(_rooms);

        // Now the List is ready to be triangulated
        Triangulate();

    }


    private void OrderRooms(List<Room> _rooms)
    {
        // Sorts rooms based on X Pos
        _rooms = _rooms.OrderBy(c => c.transform.position.x).ToList();

        CleanRooms(_rooms);
    }


    private void CleanRooms(List<Room> _rooms)
    {
        foreach (Room room in _rooms)
        {
            // 0 = small 1 = med 2 = large...
            if(room.GetRoomSize() != 0)
            {
                // Add med & large
                orderedRooms.Add(room);
            }
        }
    }


    private void Triangulate()
    {
        triangles = new List<List<Vector3>>();
        
        for (int i = 0; i < orderedRooms.Count; i++)
        {
            bool foundNN = false; // Nearest Neighbours

            List<Vector3> pos = new List<Vector3>();
            //Find 2 nearest neighbours
            while(!foundNN)
            {
                float nearestPoint = 1000.0f;

                // Check 2 lower and 2 greater
                if(i > 1 && i < (orderedRooms.Count - 2))
                {
                    List<Vector3> nearestXPositions = new List<Vector3>();

                    for (int j = (i - 2); j <= (i + 2); j++)
                    {
                        if (j == i)
                            continue;

                            pos.Add(orderedRooms[j].transform.position);
                    }

                    pos = pos.OrderBy(x => Vector3.Distance(x, orderedRooms[i].transform.position)).ToList();

                    //pos.RemoveRange(2, 2);

                    Debug.Log(pos.Count);

                    foundNN = true;

                    triangles.Add(pos);
                }

                // search the first 4
                else if(i <= 2)
                {
                    List<Vector3> nearestXPositions = new List<Vector3>();

                    for (int j = 0; j <= 4; j++)
                    {
                        if (j == i)
                            continue;

                        pos.Add(orderedRooms[j].transform.position);
                    }

                    pos = pos.OrderBy(x => Vector3.Distance(x, orderedRooms[i].transform.position)).ToList();

                    pos.RemoveRange(2, 2);

                    Debug.Log(pos.Count);

                    foundNN = true;

                    triangles.Add(pos);
                }

                else if(i >= (orderedRooms.Count - 2))
                {
                    // Check last four spots
                    Debug.Log("Check last four spots");
                    foundNN = true;
                }
            }
        }
    }*/


    // WORKING TRIANGLUATION

    [SerializeField] Transform point1;
    [SerializeField] Transform point2;
    [SerializeField] Transform point3;
    [SerializeField] Transform point4;
    [SerializeField] Transform POINT4;

    private Vector3 cross;
    private bool pointFound;

    private Vector3 point;
    private float distance;

    private void Start()
    {
        if (CheckPoint(point1.position, point2.position, point3.position, point4.position))
            Debug.Log("Crosses!");

        else
            Debug.Log("No point found...");
    }


    private bool CheckPoint(Vector3 A1, Vector3 A2, Vector3 B1, Vector3 B2)
    {
        float tmp = (B2.x - B1.x) * (A2.z - A1.z) - (B2.z - B1.z) * (A2.x - A1.x);

        if(tmp == 0)
        {
            return false;
        }

        float mu = ((A1.x - B1.x) * (A2.z - A1.z) - (A1.z - B1.z) * (A2.x - A1.x)) / tmp;

        cross = new Vector3(B1.x + (B2.x - B1.x) * mu, 0.0f, B1.z + (B2.z - B1.z) * mu);

        distance = Vector3.Distance(A1, cross);

        pointFound = true;

        return true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        if (pointFound)
        {
            Gizmos.DrawWireSphere(cross, 20.0f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(point1.position, point2.position);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(point3.position, point4.position);

            //Gizmos.DrawSphere(point, distance);
        }
    }
}
