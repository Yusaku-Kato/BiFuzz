using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ExeTime : MonoBehaviour
{
    string filePath = Path.Combine(Application.dataPath, "Logs", "exe_time.csv");
    int exeTime = 2592000;

    // Start is called before the first frame update
    void Start()
    {
        if (!File.Exists(filePath))
        {
            
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine(0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int currentValue;
        string fileContent = File.ReadAllText(filePath);
        currentValue = int.Parse(fileContent) + 1;
        File.WriteAllText(filePath, currentValue.ToString());
        if (currentValue == exeTime)
        {
            EditorApplication.isPlaying = false;
        }
    }
}
