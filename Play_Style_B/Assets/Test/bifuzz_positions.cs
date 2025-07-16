using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class PositionLogger : MonoBehaviour
{
    private string fuzzTypePath;
    private string positionsPath;

    void Start()
    {
        
        fuzzTypePath = Path.Combine(Application.dataPath, "Logs", "fuzz_type.csv");
        positionsPath = Path.Combine(Application.dataPath, "Logs", "positions.csv");

        
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Logs"));
        using (StreamWriter writer = new StreamWriter(positionsPath, true))
        {
            
            writer.WriteLine($"Scene started");
        }
    }

    void FixedUpdate()
    {
        
        //string lastLine = GetLastLine(fuzzTypePath);
        //if (lastLine == "local")
        //{
        
        //    Vector3 position = transform.position;

        
        //    LogPosition(position);
        //}
        
        Vector3 position = transform.position;

        
        LogPosition(position);
    }

    string GetLastLine(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        string lastLine = null;
        using (StreamReader reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                lastLine = reader.ReadLine();
            }
        }
        return lastLine;
    }

    void LogPosition(Vector3 position)
    {
        using (StreamWriter writer = new StreamWriter(positionsPath, true))
        {
            
            writer.WriteLine($"{position.x},{position.y},{position.z}");
        }
    }
}
