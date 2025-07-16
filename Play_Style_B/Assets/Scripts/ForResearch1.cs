using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

public class NavMeshTriangle : MonoBehaviour
{
    public NavMeshAgent agent; 
    NavMeshTriangulation triangulation;
    int polyNum;
    bool[] coverage; 
    public double triangleCoverage;
    float areaAll = 0;
    float areaNow = 0;

    private void Start()
    {
        triangulation = NavMesh.CalculateTriangulation(); 
        polyNum = triangulation.indices.Length / 3;
        Debug.Log("NavMesh has " + polyNum + " polygons.");

        coverage = new bool[triangulation.indices.Length / 3];

        
        for (int i = 0; i < triangulation.indices.Length; i += 3)
        {
            int index0 = triangulation.indices[i];
            int index1 = triangulation.indices[i + 1];
            int index2 = triangulation.indices[i + 2];

            Vector3 vertex0 = triangulation.vertices[index0];
            Vector3 vertex1 = triangulation.vertices[index1];
            Vector3 vertex2 = triangulation.vertices[index2];

            
            float length0 = Vector3.Distance(vertex0, vertex1);
            float length1 = Vector3.Distance(vertex1, vertex2);
            float length2 = Vector3.Distance(vertex2, vertex0);

            
            float x = (length0 + length1 + length2) / 2f;
            float area = Mathf.Sqrt(x * (x - length0) * (x - length1) * (x - length2));
            areaAll += area;
        }
        
    }

    void Update()
    {
        if (agent.isOnNavMesh) 
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(agent.transform.position, out hit, 0.1f, NavMesh.AllAreas)) 
            {
                
                for (int i = 0; i < triangulation.indices.Length; i += 3) 
                {
                    int index0 = triangulation.indices[i];
                    int index1 = triangulation.indices[i + 1];
                    int index2 = triangulation.indices[i + 2];

                    Vector3 vertex0 = triangulation.vertices[index0];
                    Vector3 vertex1 = triangulation.vertices[index1];
                    Vector3 vertex2 = triangulation.vertices[index2];

                    if (IsPointInTriangle(hit.position, vertex0, vertex1, vertex2)) 
                    {
                        
                        if(coverage[i / 3] != true)
                        {
                            
                            float length0 = Vector3.Distance(vertex0, vertex1);
                            float length1 = Vector3.Distance(vertex1, vertex2);
                            float length2 = Vector3.Distance(vertex2, vertex0);
                            
                            float x = (length0 + length1 + length2) / 2f;
                            float area = Mathf.Sqrt(x * (x - length0) * (x - length1) * (x - length2));
                            areaNow += area;
                            
                        }

                        

                        
                        coverage[i / 3] = true;
                        
                        
                        //triangleCoverage = (double)coverage.Count(b => b) / coverage.Length * 100;
                        

                        
                        
                        triangleCoverage = (double)(100 * (areaNow / areaAll));
                        Debug.Log("Coverage rate " + triangleCoverage + " %"); 
                        
                        break;
                    }
                }
            }
        }
    }

    bool IsPointInTriangle(Vector3 p, Vector3 p0, Vector3 p1, Vector3 p2) 
    {
        float area = 0.5f * (-p1.z * p2.x + p0.z * (-p1.x + p2.x) + p0.x * (p1.z - p2.z) + p1.x * p2.z);
        float s = 1 / (2 * area) * (p0.z * p2.x - p0.x * p2.z + (p2.z - p0.z) * p.x + (p0.x - p2.x) * p.z);
        float t = 1 / (2 * area) * (p0.x * p1.z - p0.z * p1.x + (p0.z - p1.z) * p.x + (p1.x - p0.x) * p.z);
        return s > 0 && t > 0 && 1 - s - t > 0;
    }
}
