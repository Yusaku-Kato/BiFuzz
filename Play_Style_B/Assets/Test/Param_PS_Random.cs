using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;

public class Param_PS_Random : MonoBehaviour
{
    
    CharacterController con;  
    Animator anim;            
    CinemachineFreeLook freeLook; 
    Param_Mutate paramMutate;
    MapCoverage mapCoverage;
    List<test_BugAlert> bugAlerts;

    
    private bool isPushedM = false;
    public GameObject freeLookCamera; 

    
    float normalSpeed = 6f;  
    float sprintSpeed = 10f; 
    float jump = 16f;        
    float gravity = 50f;     

    
    Vector3 moveDirection = Vector3.zero; 
    public bool isAttacking = false;      
    Vector3 startPos;                     

    
    public float v;  
    public float h;  

    
    private Vector3 cameraRight; 
    private bool jumpPressed;    
    private float speed;         
    private List<string[]> csvData;
    private int count = 0;

    private float nearestItem = Mathf.Infinity; 
    private string[] retrievedItems; 
    private string itemFilePath = Application.dataPath + "/Logs/retrieved_items.csv"; 

    public void PlayerStart()
    {
        
        con = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        paramMutate = GameObject.Find("Param_Mutate").GetComponent<Param_Mutate>();
        mapCoverage = GetComponent<MapCoverage>();
        bugAlerts = new List<test_BugAlert>(FindObjectsOfType<test_BugAlert>());

        
        freeLookCamera = GameObject.Find("FreeLook Camera");
        freeLook = freeLookCamera.GetComponent<CinemachineFreeLook>();

        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        
        startPos = transform.position;

        
        mapCoverage.visitedPositions.Clear();
        string serializedPositions = PlayerPrefs.GetString("visitedPositions", "");
        if (!string.IsNullOrEmpty(serializedPositions))
        {
            string[] positions = serializedPositions.Split(';');
            foreach (var position in positions)
            {
                string[] coordinates = position.Split(',');
                float x = float.Parse(coordinates[0]);
                float y = float.Parse(coordinates[1]);
                mapCoverage.visitedPositions.Add(new Vector2(x, y));
            }
        }

        string csvFilePath = Path.Combine(Application.dataPath, "Logs", "player_data_mutated.csv");
        
        if (File.Exists(csvFilePath))
        {
            csvData = new List<string[]>();
            using (StreamReader sr = new StreamReader(csvFilePath))
            {
                string line;
                
                sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    csvData.Add(line.Split(','));
                }
            }
        }
        else
        {
            Debug.LogError("CSV file not found: " + csvFilePath);
        }

        
        if (!File.Exists(itemFilePath)) 
        {
            using (StreamWriter sw = new StreamWriter(itemFilePath, false))
            {
                sw.WriteLine("Name,Count"); 
                Debug.Log("Created item log file and set column names");
            }
        }
        else
        {
            List<string> itemList = new List<string>(); 
            string[] lines = File.ReadAllLines(itemFilePath); 
            for (int i = 1; i < lines.Length; i++) 
            {
                string[] columns = lines[i].Split(','); 
                if (columns.Length > 0) 
                {
                    itemList.Add(columns[0]);
                }
            }

            retrievedItems = itemList.Distinct().ToArray(); 
            Debug.Log("Loaded from item log file");
        }
    }

    public void PlayerUpdate()
    {
        
        speed = float.Parse(csvData[count][1]);
        v = float.Parse(csvData[count][2]);
        h = float.Parse(csvData[count][3]);
        Vector3 cameraForward = new Vector3(float.Parse(csvData[count][4]), float.Parse(csvData[count][5]), float.Parse(csvData[count][6]));
        Vector3 cameraRight = new Vector3(float.Parse(csvData[count][7]), float.Parse(csvData[count][8]), float.Parse(csvData[count][9]));
        jumpPressed = csvData[count][10] == "1";
        Vector3 moveZ = cameraForward * v * speed;
        Vector3 moveX = cameraRight * h * speed;

        if (con.isGrounded)
        {
            moveDirection = moveZ + moveX;
            if (jumpPressed)
            {
                moveDirection.y = jump;
            }
        }
        else
        {
            moveDirection = moveZ + moveX + new Vector3(0, moveDirection.y, 0);
            moveDirection.y -= gravity * Time.deltaTime;
        }

        anim.SetFloat("MoveSpeed", (moveZ + moveX).magnitude);
        transform.LookAt(transform.position + moveZ + moveX);
        con.Move(moveDirection * Time.deltaTime);
        count++;

        
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item"); 
        Vector3 positionThis = this.transform.position; 
        foreach (GameObject item in items) 
        {
            if (retrievedItems == null || !retrievedItems.Contains(item.name))
            {
                float distance = Vector3.Distance(positionThis, item.transform.position);
                if (distance <= 1f) 
                {
                    using (StreamWriter sw = new StreamWriter(itemFilePath, true))
                    {
                        sw.WriteLine($"{item.name},{count}"); 
                        Debug.Log($"Item {item.name} has been added. Count: {count}");
                    }
                }
                if (distance < nearestItem) 
                {
                    nearestItem = distance;
                }
            }
        }

        foreach (var bugAlert in bugAlerts) 
        {
            
            if (bugAlert.isPlayerCollidered)
            {
                
                int i = Random.Range(0, 8);
                switch (i)
                {
                    case 0:
                        paramMutate.mutateNum = Random.Range(1, 101); 
                        break;
                    case 1:
                        paramMutate.mutateRow = Random.Range(0, 101); 
                        break;
                    case 2:
                        paramMutate.runRatio = Random.Range(0, 101);
                        break;
                    case 3:
                        paramMutate.v1Ratio = Random.Range(0, 101);
                        break;
                    case 4:
                        paramMutate.h0Ratio = Random.Range(0, 101);
                        break;
                    case 5:
                        paramMutate.forwardXBias = Random.Range(0, 101);
                        break;
                    case 6:
                        paramMutate.isSameSignXZ = Random.Range(0, 101);
                        break;
                    case 7:
                        paramMutate.jumpRatio = Random.Range(0, 101);
                        break;
                    case 8:
                        paramMutate.coverageDeg = Random.Range(0, 101);
                        break;
                    case 9:
                        paramMutate.itemDeg = Random.Range(0, 101);
                        break;
                    case 10:
                        paramMutate.interactionDeg = Random.Range(0, 101);
                        break;
                    case 11:
                        paramMutate.itemPri = Random.Range(0, 101);
                        break;
                    case 12:
                        paramMutate.decisionVel = Random.Range(0, 101);
                        break;
                    case 13:
                        paramMutate.directionAcc = Random.Range(0, 101);
                        break;
                    case 14:
                        paramMutate.avoidInc = Random.Range(0, 101);
                        break;
                }
                
                if (!File.Exists(Application.dataPath + "/Logs/save_data.csv"))
                {
                    using (StreamWriter writer = new StreamWriter(Application.dataPath + "/Logs/save_data.csv", false))
                    {
                        writer.WriteLine("RestartCount,MapCoverage,BugObject");
                        writer.WriteLine($"{PlayerPrefs.GetInt("restartCount", 0)},{mapCoverage.coveragePercentage},{bugAlert.transform.parent.gameObject.name}");
                    }
                    Debug.Log("save_data.csv file created and initial data added.");
                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(Application.dataPath + "/Logs/save_data.csv", true))
                    {
                        writer.WriteLine($"{PlayerPrefs.GetInt("restartCount", 0)},{mapCoverage.coveragePercentage},{bugAlert.transform.parent.gameObject.name}");
                    }
                    Debug.Log("save_data.csv file already exists.");
                }
                
                PlayerPrefs.SetInt("mutateNum", paramMutate.mutateNum);
                PlayerPrefs.SetInt("mutateRow", paramMutate.mutateRow);
                PlayerPrefs.SetInt("runRatio", paramMutate.runRatio);
                PlayerPrefs.SetInt("v1Ratio", paramMutate.v1Ratio);
                PlayerPrefs.SetInt("h0Ratio", paramMutate.h0Ratio);
                PlayerPrefs.SetInt("forwardXBias", paramMutate.forwardXBias);
                PlayerPrefs.SetInt("isSameSignXZ", paramMutate.isSameSignXZ);
                PlayerPrefs.SetInt("jumpRatio", paramMutate.jumpRatio);
                PlayerPrefs.SetInt("coverageDeg", paramMutate.coverageDeg);
                PlayerPrefs.SetInt("itemDeg", paramMutate.itemDeg);
                PlayerPrefs.SetInt("interactionDeg", paramMutate.interactionDeg);
                PlayerPrefs.SetInt("itemPri", paramMutate.itemPri);
                PlayerPrefs.SetInt("decisionVel", paramMutate.decisionVel);
                PlayerPrefs.SetInt("directionAcc", paramMutate.directionAcc);
                PlayerPrefs.SetInt("avoidInc", paramMutate.avoidInc);
                PlayerPrefs.SetInt("restartCount", PlayerPrefs.GetInt("restartCount", 0) + 1);
                List<string> positions = new List<string>();
                foreach (var position in mapCoverage.visitedPositions)
                {
                    positions.Add(position.x + "," + position.y);
                }
                string serializedPositions = string.Join(";", positions);
                PlayerPrefs.SetString("visitedPositions", serializedPositions);
                PlayerPrefs.Save(); 
                if (PlayerPrefs.GetInt("restartCount", 5) < 100){
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                else
                {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            }
        }

        
        if (count == csvData.Count)
        {
            
            int i = Random.Range(0, 8);
            switch (i)
            {
                case 0:
                    paramMutate.mutateNum = Random.Range(1, 101); 
                    break;
                case 1:
                    paramMutate.mutateRow = Random.Range(0, 101); 
                    break;
                case 2:
                    paramMutate.runRatio = Random.Range(0, 101);
                    break;
                case 3:
                    paramMutate.v1Ratio = Random.Range(0, 101);
                    break;
                case 4:
                    paramMutate.h0Ratio = Random.Range(0, 101);
                    break;
                case 5:
                    paramMutate.forwardXBias = Random.Range(0, 101);
                    break;
                case 6:
                    paramMutate.isSameSignXZ = Random.Range(0, 101);
                    break;
                case 7:
                    paramMutate.jumpRatio = Random.Range(0, 101);
                    break;
                case 8:
                    paramMutate.coverageDeg = Random.Range(0, 101);
                    break;
                case 9:
                    paramMutate.itemDeg = Random.Range(0, 101);
                    break;
                case 10:
                    paramMutate.interactionDeg = Random.Range(0, 101);
                    break;
                case 11:
                    paramMutate.itemPri = Random.Range(0, 101);
                    break;
                case 12:
                    paramMutate.decisionVel = Random.Range(0, 101);
                    break;
                case 13:
                    paramMutate.directionAcc = Random.Range(0, 101);
                    break;
                case 14:
                    paramMutate.avoidInc = Random.Range(0, 101);
                    break;
            }
            
            if (!File.Exists(Application.dataPath + "/Logs/save_data.csv"))
            {
                using (StreamWriter writer = new StreamWriter(Application.dataPath + "/Logs/save_data.csv", false))
                {
                    writer.WriteLine("RestartCount,MapCoverage,BugObject");
                    writer.WriteLine($"{PlayerPrefs.GetInt("restartCount", 0)},{mapCoverage.coveragePercentage},");
                }
                Debug.Log("save_data.csv file created and initial data added.");
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(Application.dataPath + "/Logs/save_data.csv", true))
                {
                    writer.WriteLine($"{PlayerPrefs.GetInt("restartCount", 0)},{mapCoverage.coveragePercentage},");
                }
                Debug.Log("save_data.csv file already exists.");
            }
            
            PlayerPrefs.SetInt("mutateNum", paramMutate.mutateNum);
            PlayerPrefs.SetInt("mutateRow", paramMutate.mutateRow);
            PlayerPrefs.SetInt("runRatio", paramMutate.runRatio);
            PlayerPrefs.SetInt("v1Ratio", paramMutate.v1Ratio);
            PlayerPrefs.SetInt("h0Ratio", paramMutate.h0Ratio);
            PlayerPrefs.SetInt("forwardXBias", paramMutate.forwardXBias);
            PlayerPrefs.SetInt("isSameSignXZ", paramMutate.isSameSignXZ);
            PlayerPrefs.SetInt("jumpRatio", paramMutate.jumpRatio);
            PlayerPrefs.SetInt("coverageDeg", paramMutate.coverageDeg);
            PlayerPrefs.SetInt("itemDeg", paramMutate.itemDeg);
            PlayerPrefs.SetInt("interactionDeg", paramMutate.interactionDeg);
            PlayerPrefs.SetInt("itemPri", paramMutate.itemPri);
            PlayerPrefs.SetInt("decisionVel", paramMutate.decisionVel);
            PlayerPrefs.SetInt("directionAcc", paramMutate.directionAcc);
            PlayerPrefs.SetInt("avoidInc", paramMutate.avoidInc);
            PlayerPrefs.SetInt("restartCount", PlayerPrefs.GetInt("restartCount", 0) + 1);
            List<string> positions = new List<string>();
            foreach (var position in mapCoverage.visitedPositions)
            {
                positions.Add(position.x + "," + position.y);
            }
            string serializedPositions = string.Join(";", positions);
            PlayerPrefs.SetString("visitedPositions", serializedPositions);
            PlayerPrefs.Save(); 
            if (PlayerPrefs.GetInt("restartCount", 5) < 100)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
    }

    
    public void StartAttack()
    {
        isAttacking = true;
    }

    
    public void EndAttack()
    {
        isAttacking = false;
    }

    
    private void ToggleMouseCursor()
    {
        
        isPushedM = !isPushedM;
        Cursor.visible = isPushedM;
        Cursor.lockState = isPushedM ? CursorLockMode.None : CursorLockMode.Locked;

        
        freeLook.m_XAxis.m_MaxSpeed = isPushedM ? 0 : 900;
        freeLook.m_YAxis.m_MaxSpeed = isPushedM ? 0 : 6;
    }
}
