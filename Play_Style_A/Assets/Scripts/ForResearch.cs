using UnityEngine;
using UnityEngine.AI;

public class NavMeshPolygonCounter : MonoBehaviour
{
    void Start()
    {
        //NavMesh navMesh = GetComponent<NavMesh>();
        //int polygonCount = navMesh.triangles.Length / 3;
        //Debug.Log("NavMesh has " + polygonCount + " polygons.");

        var navMesh = NavMesh.CalculateTriangulation();
        Vector3[] vertices = navMesh.vertices;
        int[] polygons = navMesh.indices;
        Debug.Log("NavMesh has " + polygons.Length / 3 + " polygons.");
        Debug.Log("NavMesh has " + vertices.Length + " vertices.");
    }
}
