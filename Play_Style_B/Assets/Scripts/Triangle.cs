using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour
{
    public int n = 6;
    void Start()
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[n * 6];
        int[] triangles = new int[(n - 1) * 12];
        for (int i = 0; i < n; i++)
        {
            
            vertices[i] = (Quaternion.Euler(0, 0, (360f / n) * i) * Vector3.up) + Vector3.back * 0.5f;
            vertices[n + i] = vertices[i] + Vector3.forward;
        }
        for (int i = 0; i < n; i++)
        {
            
            vertices[n * 2 + i * 4 + 0] = vertices[0 + i];
            vertices[n * 2 + i * 4 + 1] = vertices[(1 + i) % n];
            vertices[n * 2 + i * 4 + 2] = vertices[n + (1 + i) % n];
            vertices[n * 2 + i * 4 + 3] = vertices[n + i];
        }
        for (int i = 0; i < n - 2; i++)
        {
            
            triangles[i * 3 + 0] = 0;
            triangles[i * 3 + 1] = 2 + i;
            triangles[i * 3 + 2] = 1 + i;
            
            triangles[(n - 2) * 3 + i * 3 + 0] = n;
            triangles[(n - 2) * 3 + i * 3 + 1] = n + 1 + i;
            triangles[(n - 2) * 3 + i * 3 + 2] = n + 2 + i;
        }// ~ (n-2)*6
        for (int i = 0; i < n; i++)
        {
            
            triangles[(n - 2) * 6 + i * 6 + 0] = n * 2 + i * 4 + 0;
            triangles[(n - 2) * 6 + i * 6 + 1] = n * 2 + i * 4 + 1;
            triangles[(n - 2) * 6 + i * 6 + 2] = n * 2 + i * 4 + 2;
            triangles[(n - 2) * 6 + i * 6 + 3] = n * 2 + i * 4 + 2;
            triangles[(n - 2) * 6 + i * 6 + 4] = n * 2 + i * 4 + 3;
            triangles[(n - 2) * 6 + i * 6 + 5] = n * 2 + i * 4 + 0;
        }// ~ (n-1)*12
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}
