using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangulation : MonoBehaviour
{
    [SerializeField] Transform point1;
    [SerializeField] Transform point2;
    [SerializeField] Transform point3;
    [SerializeField] Transform point4;

    private Vector3 cross;
    private bool pointFound;

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

        pointFound = true;

        return true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        if (pointFound)
        {
            Gizmos.DrawWireSphere(cross, 1.0f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(point1.position, point2.position);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(point3.position, point4.position);
        }
    }
}
