using UnityEngine;
using System.Collections.Generic;

public class MapCoverage : MonoBehaviour
{
    
    public Vector2 mapMin = new Vector2(-50, -50);
    public Vector2 mapMax = new Vector2(50, 50);

    private float mapArea;
    private Vector2 previousPosition;

    
    public HashSet<Vector2> visitedPositions = new HashSet<Vector2>();
    
    public float coveragePercentage;

    void Start()
    {
        
        mapArea = (mapMax.x - mapMin.x) * (mapMax.y - mapMin.y);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 playerPosition = player.transform.position;
            previousPosition = new Vector2(Mathf.Round(playerPosition.x), Mathf.Round(playerPosition.z));
            visitedPositions.Add(previousPosition);
        }
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 playerPosition = player.transform.position;
            Vector2 currentPosition = new Vector2(Mathf.Round(playerPosition.x), Mathf.Round(playerPosition.z));

            
            if (currentPosition != previousPosition)
            {
                
                AddPositionsOnLine(previousPosition, currentPosition);

                
                previousPosition = currentPosition;
            }
        }

        
        coveragePercentage = CalculateCoveragePercentage();
        //Debug.Log("Map Coverage: " + coveragePercentage + "%");
    }

    void AddPositionsOnLine(Vector2 start, Vector2 end)
    {
        int steps = Mathf.CeilToInt(Vector2.Distance(start, end) / 0.1f); 
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 position = Vector2.Lerp(start, end, t);
            visitedPositions.Add(new Vector2(Mathf.Round(position.x), Mathf.Round(position.y)));
        }
    }

    float CalculateCoveragePercentage()
    {
        
        float visitedArea = visitedPositions.Count;
        
        float coveragePercentage = (visitedArea / mapArea) * 100f;
        return coveragePercentage;
    }
}
