using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;

public class Param_PS_V1 : MonoBehaviour
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

    private bool isCollidingWithBorder = false; 
    private int frameCounter = 0;

    public void PlayerStart()
    {
        Debug.Log("Number of restarts: " + PlayerPrefs.GetInt("restartCount", -1));
        
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
        
        if (isCollidingWithBorder)
        {
            frameCounter++; 

            if (frameCounter >= 300) 
            {
                if (frameCounter == 300)
                {
                    Debug.Log("Contact for more than 300 frames!");
                }
                string csvFilePath = Path.Combine(Application.dataPath, "Logs", "player_data_mutated.csv");
                if (File.Exists(csvFilePath)) 
                {
                    csvData = new List<string[]>();
                    using (StreamReader sr = new StreamReader(csvFilePath))
                    {
                        string line;
                        string header = sr.ReadLine(); 
                        csvData.Add(header.Split(',')); 

                        int currentRow = 0;
                        while ((line = sr.ReadLine()) != null)
                        {
                            var rowData = line.Split(',');

                            if (currentRow >= count)
                            {
                                rowData[4] = (-float.Parse(rowData[4])).ToString(); // CameraForwardX
                                rowData[6] = (-float.Parse(rowData[6])).ToString(); // CameraForwardZ
                                rowData[7] = (-float.Parse(rowData[7])).ToString(); // CameraRightX
                                rowData[9] = (-float.Parse(rowData[9])).ToString(); // CameraRightZ
                            }

                            csvData.Add(rowData);
                            currentRow++;
                        }
                    }

                    using (StreamWriter sw = new StreamWriter(csvFilePath)) 
                    {
                        foreach (var row in csvData)
                        {
                            sw.WriteLine(string.Join(",", row));
                        }
                    }

                    Debug.Log("Reached the map edge, updated CSV file data: " + csvFilePath);
                }
                else
                {
                    Debug.LogError("Reached the map edge, but CSV file not found: " + csvFilePath);
                }
            }
        }

        
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
                
                float nearestItemRecent = PlayerPrefs.GetFloat("nearestItemRecent", nearestItem);
                
                if (nearestItem < nearestItemRecent)
                {
                    nearestItemRecent = nearestItem;
                    
                    PlayerPrefs.SetInt("repCount", 0);
                    PlayerPrefs.SetInt("near_mutateNum", paramMutate.mutateNum);
                    PlayerPrefs.SetInt("near_mutateRow", paramMutate.mutateRow);
                    PlayerPrefs.SetInt("near_runRatio", paramMutate.runRatio);
                    PlayerPrefs.SetInt("near_v1Ratio", paramMutate.v1Ratio);
                    PlayerPrefs.SetInt("near_h0Ratio", paramMutate.h0Ratio);
                    PlayerPrefs.SetInt("near_forwardXBias", paramMutate.forwardXBias);
                    PlayerPrefs.SetInt("near_isSameSignXZ", paramMutate.isSameSignXZ);
                    PlayerPrefs.SetInt("near_jumpRatio", paramMutate.jumpRatio);
                    PlayerPrefs.SetInt("near_coverageDeg", paramMutate.coverageDeg);
                    PlayerPrefs.SetInt("near_itemDeg", paramMutate.itemDeg);
                    PlayerPrefs.SetInt("near_interactionDeg", paramMutate.interactionDeg);
                    PlayerPrefs.SetInt("near_itemPri", paramMutate.itemPri);
                    PlayerPrefs.SetInt("near_decisionVel", paramMutate.decisionVel);
                    PlayerPrefs.SetInt("near_directionAcc", paramMutate.directionAcc);
                    PlayerPrefs.SetInt("near_avoidInc", paramMutate.avoidInc);
                    PlayerPrefs.SetInt("fixNum", PlayerPrefs.GetInt("fixNum", 1) * 2);
                    Debug.Log($"Minimum distance updated. fixNum = {PlayerPrefs.GetInt("fixNum", 1)}, repCount = {PlayerPrefs.GetInt("repCount", -1)}");
                }
                else
                {
                    Debug.Log($"Minimum distance not updated. fixNum = {PlayerPrefs.GetInt("fixNum", 1)}, repCount = {PlayerPrefs.GetInt("repCount", -1)}");
                }
                if (PlayerPrefs.GetInt("repCount", 5) <  PlayerPrefs.GetInt("fixNum", 1))
                {
                    Debug.Log("Mutation source fixed");
                    
                    paramMutate.mutateNum = PlayerPrefs.GetInt("near_mutateNum", paramMutate.mutateNum);
                    paramMutate.mutateRow = PlayerPrefs.GetInt("near_mutateRow", paramMutate.mutateRow);
                    paramMutate.runRatio = PlayerPrefs.GetInt("near_runRatio", paramMutate.runRatio);
                    paramMutate.v1Ratio = PlayerPrefs.GetInt("near_v1Ratio", paramMutate.v1Ratio);
                    paramMutate.h0Ratio = PlayerPrefs.GetInt("near_h0Ratio", paramMutate.h0Ratio);
                    paramMutate.forwardXBias = PlayerPrefs.GetInt("near_forwardXBias", paramMutate.forwardXBias);
                    paramMutate.isSameSignXZ = PlayerPrefs.GetInt("near_isSameSignXZ", paramMutate.isSameSignXZ);
                    paramMutate.jumpRatio = PlayerPrefs.GetInt("near_jumpRatio", paramMutate.jumpRatio);
                    paramMutate.coverageDeg = PlayerPrefs.GetInt("near_coverageDeg", paramMutate.coverageDeg);
                    paramMutate.itemDeg = PlayerPrefs.GetInt("near_itemDeg", paramMutate.itemDeg);
                    paramMutate.interactionDeg = PlayerPrefs.GetInt("near_interactionDeg", paramMutate.interactionDeg);
                    paramMutate.itemPri = PlayerPrefs.GetInt("near_itemPri", paramMutate.itemPri);
                    paramMutate.decisionVel = PlayerPrefs.GetInt("near_decisionVel", paramMutate.decisionVel);
                    paramMutate.directionAcc = PlayerPrefs.GetInt("near_directionAcc", paramMutate.directionAcc);
                    paramMutate.avoidInc = PlayerPrefs.GetInt("near_avoidInc", paramMutate.avoidInc);
                    
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
                    
                    PlayerPrefs.SetInt("repCount", PlayerPrefs.GetInt("repCount", 5) + 1);
                }
                else
                {
                    Debug.Log("Mutation source not fixed");
                    if (PlayerPrefs.GetInt("repCount", 5) == PlayerPrefs.GetInt("fixNum", 1))
                    {
                        Debug.Log("Restored to initial parameters");
                        paramMutate.mutateNum = paramMutate.orig_mutateNum;
                        paramMutate.mutateRow = paramMutate.orig_mutateRow;
                        paramMutate.runRatio = paramMutate.orig_runRatio;
                        paramMutate.v1Ratio = paramMutate.orig_v1Ratio;
                        paramMutate.h0Ratio = paramMutate.orig_h0Ratio;
                        paramMutate.forwardXBias = paramMutate.orig_forwardXBias;
                        paramMutate.isSameSignXZ = paramMutate.orig_isSameSignXZ;
                        paramMutate.jumpRatio = paramMutate.orig_jumpRatio;
                        paramMutate.coverageDeg = paramMutate.orig_coverageDeg;
                        paramMutate.itemDeg = paramMutate.orig_itemDeg;
                        paramMutate.interactionDeg = paramMutate.orig_interactionDeg;
                        paramMutate.itemPri = paramMutate.orig_itemPri;
                        paramMutate.decisionVel = paramMutate.orig_decisionVel;
                        paramMutate.directionAcc = paramMutate.orig_directionAcc;
                        paramMutate.avoidInc = paramMutate.orig_avoidInc;
                        PlayerPrefs.SetInt("fixNum", 1);
                    }
                    
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
                    
                    PlayerPrefs.SetInt("repCount", PlayerPrefs.GetInt("repCount", 5) + 1);
                }
                Debug.Log(PlayerPrefs.GetInt("repCount", 5));
                
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
                if (nearestItemRecent <= 1f) 
                {
                    PlayerPrefs.SetFloat("nearestItemRecent", Mathf.Infinity);
                    Debug.Log("fixNum initialized because an item was collected.");
                    PlayerPrefs.SetInt("fixNum", 1);
                }
                else
                {
                    PlayerPrefs.SetFloat("nearestItemRecent", nearestItemRecent);
                }
                PlayerPrefs.SetInt("restartCount", PlayerPrefs.GetInt("restartCount", 0) + 1);
                Debug.Log("restartCount incremented: " + PlayerPrefs.GetInt("restartCount", -1));
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
            
            float nearestItemRecent = PlayerPrefs.GetFloat("nearestItemRecent", nearestItem);
            
            if (nearestItem < nearestItemRecent)
            {
                nearestItemRecent = nearestItem;
                
                PlayerPrefs.SetInt("repCount", 0);
                PlayerPrefs.SetInt("near_mutateNum", paramMutate.mutateNum);
                PlayerPrefs.SetInt("near_mutateRow", paramMutate.mutateRow);
                PlayerPrefs.SetInt("near_runRatio", paramMutate.runRatio);
                PlayerPrefs.SetInt("near_v1Ratio", paramMutate.v1Ratio);
                PlayerPrefs.SetInt("near_h0Ratio", paramMutate.h0Ratio);
                PlayerPrefs.SetInt("near_forwardXBias", paramMutate.forwardXBias);
                PlayerPrefs.SetInt("near_isSameSignXZ", paramMutate.isSameSignXZ);
                PlayerPrefs.SetInt("near_jumpRatio", paramMutate.jumpRatio);
                PlayerPrefs.SetInt("near_coverageDeg", paramMutate.coverageDeg);
                PlayerPrefs.SetInt("near_itemDeg", paramMutate.itemDeg);
                PlayerPrefs.SetInt("near_interactionDeg", paramMutate.interactionDeg);
                PlayerPrefs.SetInt("near_itemPri", paramMutate.itemPri);
                PlayerPrefs.SetInt("near_decisionVel", paramMutate.decisionVel);
                PlayerPrefs.SetInt("near_directionAcc", paramMutate.directionAcc);
                PlayerPrefs.SetInt("near_avoidInc", paramMutate.avoidInc);
                PlayerPrefs.SetInt("fixNum", PlayerPrefs.GetInt("fixNum", 1) * 2);
                Debug.Log($"Minimum distance updated. fixNum = {PlayerPrefs.GetInt("fixNum", 1)}, repCount = {PlayerPrefs.GetInt("repCount", -1)}");
            }
            else
            {
                Debug.Log($"Minimum distance not updated. fixNum = {PlayerPrefs.GetInt("fixNum", 1)}, repCount = {PlayerPrefs.GetInt("repCount", -1)}");
            }
            if (PlayerPrefs.GetInt("repCount", 5) < PlayerPrefs.GetInt("fixNum", 1))
            {
                Debug.Log("Mutation source fixed");
                
                paramMutate.mutateNum = PlayerPrefs.GetInt("near_mutateNum", paramMutate.mutateNum);
                paramMutate.mutateRow = PlayerPrefs.GetInt("near_mutateRow", paramMutate.mutateRow);
                paramMutate.runRatio = PlayerPrefs.GetInt("near_runRatio", paramMutate.runRatio);
                paramMutate.v1Ratio = PlayerPrefs.GetInt("near_v1Ratio", paramMutate.v1Ratio);
                paramMutate.h0Ratio = PlayerPrefs.GetInt("near_h0Ratio", paramMutate.h0Ratio);
                paramMutate.forwardXBias = PlayerPrefs.GetInt("near_forwardXBias", paramMutate.forwardXBias);
                paramMutate.isSameSignXZ = PlayerPrefs.GetInt("near_isSameSignXZ", paramMutate.isSameSignXZ);
                paramMutate.jumpRatio = PlayerPrefs.GetInt("near_jumpRatio", paramMutate.jumpRatio);
                paramMutate.coverageDeg = PlayerPrefs.GetInt("near_coverageDeg", paramMutate.coverageDeg);
                paramMutate.itemDeg = PlayerPrefs.GetInt("near_itemDeg", paramMutate.itemDeg);
                paramMutate.interactionDeg = PlayerPrefs.GetInt("near_interactionDeg", paramMutate.interactionDeg);
                paramMutate.itemPri = PlayerPrefs.GetInt("near_itemPri", paramMutate.itemPri);
                paramMutate.decisionVel = PlayerPrefs.GetInt("near_decisionVel", paramMutate.decisionVel);
                paramMutate.directionAcc = PlayerPrefs.GetInt("near_directionAcc", paramMutate.directionAcc);
                paramMutate.avoidInc = PlayerPrefs.GetInt("near_avoidInc", paramMutate.avoidInc);
                
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
                
                PlayerPrefs.SetInt("repCount", PlayerPrefs.GetInt("repCount", 5) + 1);
            }
            else
            {
                Debug.Log("Mutation source not fixed");
                if (PlayerPrefs.GetInt("repCount", 5) == PlayerPrefs.GetInt("fixNum", 1))
                {
                    Debug.Log("Restored to initial parameters");
                    paramMutate.mutateNum = paramMutate.orig_mutateNum;
                    paramMutate.mutateRow = paramMutate.orig_mutateRow;
                    paramMutate.runRatio = paramMutate.orig_runRatio;
                    paramMutate.v1Ratio = paramMutate.orig_v1Ratio;
                    paramMutate.h0Ratio = paramMutate.orig_h0Ratio;
                    paramMutate.forwardXBias = paramMutate.orig_forwardXBias;
                    paramMutate.isSameSignXZ = paramMutate.orig_isSameSignXZ;
                    paramMutate.jumpRatio = paramMutate.orig_jumpRatio;
                    paramMutate.coverageDeg = paramMutate.orig_coverageDeg;
                    paramMutate.itemDeg = paramMutate.orig_itemDeg;
                    paramMutate.interactionDeg = paramMutate.orig_interactionDeg;
                    paramMutate.itemPri = paramMutate.orig_itemPri;
                    paramMutate.decisionVel = paramMutate.orig_decisionVel;
                    paramMutate.directionAcc = paramMutate.orig_directionAcc;
                    paramMutate.avoidInc = paramMutate.orig_avoidInc;
                    PlayerPrefs.SetInt("fixNum", 1);
                }
                
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
                
                PlayerPrefs.SetInt("repCount", PlayerPrefs.GetInt("repCount", 5) + 1);
            }
            Debug.Log(PlayerPrefs.GetInt("repCount", 5));
            
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
            if (nearestItemRecent <= 1f) 
            {
                PlayerPrefs.SetFloat("nearestItemRecent", Mathf.Infinity);
                Debug.Log("fixNum initialized because an item was collected.");
                PlayerPrefs.SetInt("fixNum", 1);
            }
            else
            {
                PlayerPrefs.SetFloat("nearestItemRecent", nearestItemRecent);
            }
            PlayerPrefs.SetInt("restartCount", PlayerPrefs.GetInt("restartCount", 0) + 1);
            Debug.Log("restartCount incremented: " + PlayerPrefs.GetInt("restartCount", -1));
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

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Border"))
        {
            isCollidingWithBorder = true;
            frameCounter = 0; 
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Border"))
        {
            isCollidingWithBorder = false;
            frameCounter = 0; 
        }
    }
}
