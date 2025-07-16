using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.IO;
using System.Linq;
using NUnit.Framework.Internal.Builders;
using UnityEngine.SceneManagement;
using System;

public class grad_local : MonoBehaviour
{
    CharacterController controller;
    Animator animator;
    CinemachineFreeLook freeLook;
    private bool isCursorLocked = true;
    public GameObject freeLookCamera;
    float normalSpeed = 6f;
    float sprintSpeed = 10f;
    float jump = 16f;
    float gravity = 50f;
    Vector3 moveDirection = Vector3.zero;
    public GameObject targetObject;
    private List<string[]> csvData;
    private int frameIndex;

    
    [SerializeField, Range(0, 99)] 
    public int cpNum1;
    [SerializeField, Range(0, 99)]
    public int cpNum2;
    [SerializeField, Range(0, 99)]
    public int cpNum3;
    [SerializeField, Range(0, 99)]
    public int cpNum4;
    [SerializeField, Range(0, 99)]
    public int cpNum5;
    [SerializeField, Range(0, 99)]
    public int cpNum6;
    [SerializeField, Range(0, 99)]
    public int cpNum7;
    [SerializeField, Range(0, 99)]
    public int cpNum8;
    [SerializeField, Range(0, 99)]
    public int cpNum9;
    [SerializeField, Range(0, 99)]
    public int cpNum10;
    [SerializeField, Range(0, 99)] 
    public int cpNum1_range;
    [SerializeField, Range(0, 99)]
    public int cpNum2_range;
    [SerializeField, Range(0, 99)]
    public int cpNum3_range;
    [SerializeField, Range(0, 99)]
    public int cpNum4_range;
    [SerializeField, Range(0, 99)]
    public int cpNum5_range;
    [SerializeField, Range(0, 99)]
    public int cpNum6_range;
    [SerializeField, Range(0, 99)]
    public int cpNum7_range;
    [SerializeField, Range(0, 99)]
    public int cpNum8_range;
    [SerializeField, Range(0, 99)]
    public int cpNum9_range;
    [SerializeField, Range(0, 99)]
    public int cpNum10_range;
    [SerializeField, Range(1, 4)] 
    public int cpNum1_dir;
    [SerializeField, Range(1, 4)]
    public int cpNum2_dir;
    [SerializeField, Range(1, 4)]
    public int cpNum3_dir;
    [SerializeField, Range(1, 4)]
    public int cpNum4_dir;
    [SerializeField, Range(1, 4)]
    public int cpNum5_dir;
    [SerializeField, Range(1, 4)]
    public int cpNum6_dir;
    [SerializeField, Range(1, 4)]
    public int cpNum7_dir;
    [SerializeField, Range(1, 4)]
    public int cpNum8_dir;
    [SerializeField, Range(1, 4)]
    public int cpNum9_dir;
    [SerializeField, Range(1, 4)]
    public int cpNum10_dir;

    
    Vector3 ss1_loc = new Vector3(170f, 48.97f, 303f);
    Vector3 ss2_loc = new Vector3(183f, 19.884f, 460f);
    Vector3 ss3_loc = new Vector3(352f, 52.008f, 285f);
    Vector3 ss4_loc = new Vector3(935f, 91.763f, 356f);
    Vector3 ss5_loc = new Vector3(544f, 19.208f, 19f);
    Vector3 ss6_loc = new Vector3(482f, 57.862f, 302f);
    Vector3 ss7_loc = new Vector3(759f, 52.687f, 471f);
    Vector3 ss8_loc = new Vector3(262f, 15.44f, 988f);
    Vector3 ss9_loc = new Vector3(844f, 71.148f, 660f);
    Vector3 ss10_loc = new Vector3(282.54f, 20.02f, 779.41f);

    Vector3[] cp1; 
    Vector3[] cp2;
    Vector3[] cp3;
    Vector3[] cp4;
    Vector3[] cp5;
    Vector3[] cp6;
    Vector3[] cp7;
    Vector3[] cp8;
    Vector3[] cp9;
    Vector3[] cp10;

    
    public struct ss
    {
        public int index;
        public Vector3 loc;

        
        public ss(int index, bool on, int pri, Vector3 loc)
        {
            this.index = index;
            this.loc = loc;
        }
    }
    ss[] strategy;

    
    Vector3 postPos = Vector3.zero;
    Vector3 nowPos = Vector3.zero;
    Vector3 fixed_nowPos = Vector3.zero;
    int avdCnt = 0;
    bool isAvding = false;
    bool isAvoidRight = false;

    
    string filePath = Path.Combine(Application.dataPath, "Logs", "fuzz.csv");

    
    [SerializeField]
    private GameObject stuckBugs;

    void Start()
    {
        
        Application.logMessageReceived += OnLogMessageReceived;

        
        if (!File.Exists(Path.Combine(Application.dataPath, "Logs", "param_local.csv")))
        {
            try
            {
                
                using (StreamWriter writer = new StreamWriter(Path.Combine(Application.dataPath, "Logs", "param_local.csv"), false))
                {
                    
                    writer.WriteLine("cpNum1,cpNum2,cpNum3,cpNum4,cpNum5,cpNum6,cpNum7,cpNum8,cpNum9,cpNum10,cpNum1_range,cpNum2_range,cpNum3_range,cpNum4_range,cpNum5_range,cpNum6_range,cpNum7_range,cpNum8_range,cpNum9_range,cpNum10_range,cpNum1_dir,cpNum2_dir,cpNum3_dir,cpNum4_dir,cpNum5_dir,cpNum6_dir,cpNum7_dir,cpNum8_dir,cpNum9_dir,cpNum10_dir");
                    writer.WriteLine($"{cpNum1},{cpNum2},{cpNum3},{cpNum4},{cpNum5},{cpNum6},{cpNum7},{cpNum8},{cpNum9},{cpNum10},{cpNum1_range},{cpNum2_range},{cpNum3_range},{cpNum4_range},{cpNum5_range},{cpNum6_range},{cpNum7_range},{cpNum8_range},{cpNum9_range},{cpNum10_range},{cpNum1_dir},{cpNum2_dir},{cpNum3_dir},{cpNum4_dir},{cpNum5_dir},{cpNum6_dir},{cpNum7_dir},{cpNum8_dir},{cpNum9_dir},{cpNum10_dir}");
                    Debug.Log($"CSV file 'fuzz.csv' created: {filePath}");
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to create CSV file: {e.Message}");
            }
        }

        List<string> paramLines = new List<string>(File.ReadAllLines(Path.Combine(Application.dataPath, "Logs", "param_local.csv")));
        string paramLastLine = paramLines[paramLines.Count - 1];
        string[] paramColumns = paramLastLine.Split(',');

        //Debug.Log("cpNum1: " + int.Parse(paramColumns[0]));
        //Debug.Log("cpNum10_dir: " + int.Parse(paramColumns[29]));
        cp1 = create_cp(int.Parse(paramColumns[0]), int.Parse(paramColumns[10]), int.Parse(paramColumns[20]), ss1_loc);
        cp2 = create_cp(int.Parse(paramColumns[1]), int.Parse(paramColumns[11]), int.Parse(paramColumns[21]), ss2_loc);
        cp3 = create_cp(int.Parse(paramColumns[2]), int.Parse(paramColumns[12]), int.Parse(paramColumns[22]), ss3_loc);
        cp4 = create_cp(int.Parse(paramColumns[3]), int.Parse(paramColumns[13]), int.Parse(paramColumns[23]), ss4_loc);
        cp5 = create_cp(int.Parse(paramColumns[4]), int.Parse(paramColumns[14]), int.Parse(paramColumns[24]), ss5_loc);
        cp6 = create_cp(int.Parse(paramColumns[5]), int.Parse(paramColumns[15]), int.Parse(paramColumns[25]), ss6_loc);
        cp7 = create_cp(int.Parse(paramColumns[6]), int.Parse(paramColumns[16]), int.Parse(paramColumns[26]), ss7_loc);
        cp8 = create_cp(int.Parse(paramColumns[7]), int.Parse(paramColumns[17]), int.Parse(paramColumns[27]), ss8_loc);
        cp9 = create_cp(int.Parse(paramColumns[8]), int.Parse(paramColumns[18]), int.Parse(paramColumns[28]), ss9_loc);
        cp10 = create_cp(int.Parse(paramColumns[9]), int.Parse(paramColumns[19]), int.Parse(paramColumns[29]), ss10_loc);

        
        List<string> csvLines = new List<string>(File.ReadAllLines(Path.Combine(Application.dataPath, "Logs", "check_points.csv")));
        for (int i = 0; i < csvLines.Count; i++)
        {
            string[] columns = csvLines[i].Split(',');
            switch (columns[0])
            {
                case "1":
                    for(int j = 0; j < cp1.Length; j++)
                    {
                        csvLines.Insert(i + 1, $"0,{cp1[j].x},{cp1[j].y},{cp1[j].z}");
                    }
                    break;
                case "2":
                    for (int j = 0; j < cp2.Length; j++)
                    {
                        csvLines.Insert(i + 1, $"0,{cp2[j].x},{cp2[j].y},{cp2[j].z}");
                    }
                    break;
                case "3":
                    for (int j = 0; j < cp3.Length; j++)
                    {
                        csvLines.Insert(i + 1, $"0,{cp3[j].x},{cp3[j].y},{cp3[j].z}");
                    }
                    break;
                case "4":
                    for (int j = 0; j < cp4.Length; j++)
                    {
                        csvLines.Insert(i + 1, $"0,{cp4[j].x},{cp4[j].y},{cp4[j].z}");
                    }
                    break;
                case "5":
                    for (int j = 0; j < cp5.Length; j++)
                    {
                        csvLines.Insert(i + 1, $"0,{cp5[j].x},{cp5[j].y},{cp5[j].z}");
                    }
                    break;
                case "6":
                    for (int j = 0; j < cp6.Length; j++)
                    {
                        csvLines.Insert(i + 1, $"0,{cp6[j].x},{cp6[j].y},{cp6[j].z}");
                    }
                    break;
                case "7":
                    for (int j = 0; j < cp7.Length; j++)
                    {
                        csvLines.Insert(i + 1, $"0,{cp7[j].x},{cp7[j].y},{cp7[j].z}");
                    }
                    break;
                case "8":
                    for (int j = 0; j < cp8.Length; j++)
                    {
                        csvLines.Insert(i + 1, $"0,{cp8[j].x},{cp8[j].y},{cp8[j].z}");
                    }
                    break;
                case "9":
                    for (int j = 0; j < cp9.Length; j++)
                    {
                        csvLines.Insert(i + 1, $"0,{cp9[j].x},{cp9[j].y},{cp9[j].z}");
                    }
                    break;
                case "10":
                    for (int j = 0; j < cp10.Length; j++)
                    {
                        csvLines.Insert(i + 1, $"0,{cp10[j].x},{cp10[j].y},{cp10[j].z}");
                    }
                    break;
            }
        }
        for (int i = csvLines.Count - 1; i >= 0; i--)
        {
            string[] columns = csvLines[i].Split(',');
            if (columns[0] == "0")
            {
                csvLines.RemoveAt(i);
            }
            else
            {
                break;
            }
        }
        
        File.WriteAllLines(Path.Combine(Application.dataPath, "Logs", "check_points.csv"), csvLines.ToArray());


        
        strategy = new ss[csvLines.Count];
        Debug.Log("=== Sorted array ===");
        for (int i = 1; i < csvLines.Count; i++)
        {
            string[] columns = csvLines[i].Split(',');
            strategy[i - 1].index = int.Parse(columns[0]);
            strategy[i - 1].loc.x = float.Parse(columns[1]);
            strategy[i - 1].loc.y = float.Parse(columns[2]);
            strategy[i - 1].loc.z = float.Parse(columns[3]);
            Debug.Log(strategy[i-1].index);
            Debug.Log(strategy[i - 1].loc);
            Debug.Log($"Index: {columns[0]}, Location: ({columns[1]},{columns[2]},{columns[3]})");
        }
        Debug.Log($"Length of strategy: {strategy.Length}");

        
        //if (stuckBugs != null)
        //{
        //    stuckBugs.SetActive(true);
        
        //}
        //else
        //{
        
        //}

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        freeLookCamera = GameObject.Find("FreeLook Camera");
        freeLook = freeLookCamera.GetComponent<CinemachineFreeLook>();
        LockCursor();

        frameIndex = 0;

        
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("speed,v,h,forwardX,forwardZ,Jump");
                Debug.Log($"CSV file 'fuzz.csv' created: {filePath}");
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to create CSV file: {e.Message}");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleCursorLock();
        }

        animator.SetBool("LeftClick", false);

        
        if (frameIndex == 108000)
        {
            Debug.Log("Timeout");

            
            try
            {
                using (StreamWriter writer = new StreamWriter(Path.Combine(Application.dataPath, "Logs", "detected_bugs.csv"), true))
                {
                    writer.WriteLine("timeout");
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to create CSV file: {e.Message}");
            }

            
            if (File.Exists(Path.Combine(Application.dataPath, "Logs", "fuzz_type.csv")))
            {
                using (StreamWriter writer = new StreamWriter(Path.Combine(Application.dataPath, "Logs", "fuzz_type.csv"), true))
                {
                    
                    string[] lines = File.ReadAllLines(Path.Combine(Application.dataPath, "Logs", "detected_bugs.csv"));
                    if (lines.Length >= 2 && lines[lines.Length - 1] == lines[lines.Length - 2])
                    {
                        writer.WriteLine("init");
                    }
                    else
                    {
                        writer.WriteLine("local");
                    }
                }
                Debug.Log("Wrote to fuzz_type.csv file.");
            }
            else
            {
                Debug.Log("Error: fuzz_type.csv file does not exist.");
                UnityEditor.EditorApplication.isPlaying = false;
            }
            MutateParam();
            Application.logMessageReceived -= OnLogMessageReceived;
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.path); 
            return;
        }

        
        if (Mathf.Abs(nowPos.x - strategy[0].loc.x) <= 0.1f && Mathf.Abs(nowPos.z - strategy[0].loc.z) <= 0.1f)
        {
            strategy = strategy.Skip(1).ToArray();
            if (strategy.Length == 1)
            {
                Debug.Log("Input finished");

                
                try
                {
                    using (StreamWriter writer = new StreamWriter(Path.Combine(Application.dataPath, "Logs", "detected_bugs.csv"), true))
                    {
                        writer.WriteLine("null");
                    }
                }
                catch (IOException e)
                {
                    Debug.LogError($"Failed to create CSV file: {e.Message}");
                }

                
                if (File.Exists(Path.Combine(Application.dataPath, "Logs", "fuzz_type.csv")))
                {
                    using (StreamWriter writer = new StreamWriter(Path.Combine(Application.dataPath, "Logs", "fuzz_type.csv"), true))
                    {
                        
                        string[] lines = File.ReadAllLines(Path.Combine(Application.dataPath, "Logs", "detected_bugs.csv"));
                        if (lines.Length >= 2 && lines[lines.Length - 1] == lines[lines.Length - 2])
                        {
                            writer.WriteLine("init");
                        }
                        else
                        {
                            writer.WriteLine("local");
                        }
                    }
                    Debug.Log("Wrote to fuzz_type.csv file.");
                }
                else
                {
                    Debug.Log("Error: fuzz_type.csv file does not exist.");
                    UnityEditor.EditorApplication.isPlaying = false;
                }
                MutateParam();
                Application.logMessageReceived -= OnLogMessageReceived;
                Scene scene = SceneManager.GetActiveScene(); 
                SceneManager.LoadScene(scene.path); 
                return;
            }
            Debug.Log("Item collected");
        }

        
        if (frameIndex % 60 == 0)
        {
            float distance = Vector3.Distance(new Vector3(postPos.x, 0, postPos.z), new Vector3(nowPos.x, 0, nowPos.z));
            if (distance < 1f)
            {
                isAvding = true;
            }
            postPos = nowPos;
        }

        Vector3 moveZ;
        Vector3 moveX;
        Vector3 direction;
        if (isAvding == false)
        {
            
            direction = new Vector3(strategy[0].loc.x - nowPos.x, 0f, strategy[0].loc.z - nowPos.z).normalized;
            moveZ = direction * 1 * sprintSpeed;
            moveX = new Vector3(0f, 0f, 0f);
        }
        else
        {
            if (avdCnt == 0) 
            {
                Vector3 forwardDirection = transform.forward;
                direction = new Vector3(-transform.forward.x, transform.forward.y, -transform.forward.z);
                moveZ = direction * 1 * sprintSpeed;
                moveX = new Vector3(0f, 0f, 0f);
                avdCnt++;
            }
            else if (avdCnt < 60) 
            {
                direction = transform.forward;
                moveZ = direction * 1 * sprintSpeed;
                moveX = new Vector3(0f, 0f, 0f);
                avdCnt++;
            }
            else if (avdCnt == 60) 
            {
                int i = UnityEngine.Random.Range(0, 2);
                if (i == 0) 
                {
                    isAvoidRight = false;
                    direction = -transform.right;
                }
                else 
                {
                    isAvoidRight = true;
                    direction = transform.right;
                }
                moveZ = direction * 1 * sprintSpeed;
                moveX = new Vector3(0f, 0f, 0f);
                avdCnt++;
            }
            else if (avdCnt < 120) 
            {
                direction = transform.forward;
                moveZ = direction * 1 * sprintSpeed;
                moveX = new Vector3(0f, 0f, 0f);
                avdCnt++;
            }
            else if (avdCnt == 120) 
            {
                if (!isAvoidRight)
                {
                    direction = -transform.right;
                }
                else
                {
                    direction = transform.right;
                }
                moveZ = direction * 1 * sprintSpeed;
                moveX = new Vector3(0f, 0f, 0f);
                avdCnt++;
            }
            else if (avdCnt < 239)
            {
                direction = transform.forward;
                moveZ = direction * 1 * sprintSpeed;
                moveX = new Vector3(0f, 0f, 0f);
                avdCnt++;
            }
            else // avdCnt == 239
            {
                direction = transform.forward;
                moveZ = direction * 1 * sprintSpeed;
                moveX = new Vector3(0f, 0f, 0f);
                avdCnt = 0;
                isAvding = false;
            }
        }

        
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                string newLine = $"10,1,0,{direction.x},{direction.z},0";
                writer.WriteLine(newLine);
                
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to write to CSV file: {e.Message}");
        }

        if (controller.isGrounded)
        {
            moveDirection = moveZ + moveX;
        }
        else
        {
            moveDirection += new Vector3(0, -gravity * Time.deltaTime, 0);
        }
        animator.SetFloat("MoveSpeed", (moveZ + moveX).magnitude);
        controller.Move(moveDirection * Time.deltaTime);
        nowPos = transform.position;
        //Debug.Log(nowPos);
        transform.LookAt(nowPos + moveZ + moveX);
        frameIndex++;
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SetFreeLookSpeed(900, 6);
        isCursorLocked = true;
    }

    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SetFreeLookSpeed(0, 0);
        isCursorLocked = false;
    }

    private void ToggleCursorLock()
    {
        if (isCursorLocked)
            UnlockCursor();
        else
            LockCursor();
    }

    private void SetFreeLookSpeed(float xSpeed, float ySpeed)
    {
        freeLook.m_XAxis.m_MaxSpeed = xSpeed;
        freeLook.m_YAxis.m_MaxSpeed = ySpeed;
    }

    private void QuitApplication()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private Vector3[] create_cp(int num, int range, int dir, Vector3 ref_point)
    {
        Vector3[] cp = new Vector3[num];
        float x;
        float z;
        switch (dir)
        {
            case 1:
                for (int i = 0; i < num; i++)
                {
                    float x_max = ref_point.x + (1024f - ref_point.x) * ((float)(range + 1) / 100f);
                    float z_max = ref_point.z + (1024f - ref_point.z) * ((float)(range + 1) / 100f);
                    x = UnityEngine.Random.Range(ref_point.x, x_max);
                    z = UnityEngine.Random.Range(ref_point.z, z_max);
                    cp[i] = new Vector3(x, 0, z);
                }
                break;
            case 2:
                for (int i = 0; i < num; i++)
                {
                    float x_min = ref_point.x - ref_point.x * ((float)(range + 1) / 100f);
                    float z_max = ref_point.z + (1024f - ref_point.z) * ((float)(range + 1) / 100f);
                    x = UnityEngine.Random.Range(x_min, ref_point.x);
                    z = UnityEngine.Random.Range(ref_point.z, z_max);
                    cp[i] = new Vector3(x, 0, z);
                }
                break;
            case 3:
                for (int i = 0; i < num; i++)
                {
                    float x_min = ref_point.x - ref_point.x * ((float)(range + 1) / 100f);
                    float z_min = ref_point.z - ref_point.z * ((float)(range + 1) / 100f);
                    x = UnityEngine.Random.Range(x_min, ref_point.x);
                    z = UnityEngine.Random.Range(z_min, ref_point.z);
                    cp[i] = new Vector3(x, 0, z);
                }
                break;
            case 4:
                for (int i = 0; i < num; i++)
                {
                    float x_max = ref_point.x + (1024f - ref_point.x) * ((float)(range + 1) / 100f);
                    float z_min = ref_point.z - ref_point.z * ((float)(range + 1) / 100f);
                    x = UnityEngine.Random.Range(ref_point.x, x_max);
                    z = UnityEngine.Random.Range(z_min, ref_point.z);
                    cp[i] = new Vector3(x, 0, z);
                }
                break;
        }
        return cp;
    }

    
    private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        
        if (condition.Contains("Bug occurred"))
        {
            Debug.Log("Bug detected");
            
            MutateParam();
            Application.logMessageReceived -= OnLogMessageReceived;
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.path); 
            return;
        }
    }

    
    private void MutateParam()
    {
        int maxNum = 100; 
        int maxRange = 100; 
        List<string> typeLines = new List<string>(File.ReadAllLines(Path.Combine(Application.dataPath, "Logs", "fuzz_type.csv")));
        string nextType = typeLines[typeLines.Count - 1];
        List<string> csvLines = new List<string>(File.ReadAllLines(Path.Combine(Application.dataPath, "Logs", "param_local.csv")));
        if (nextType == "init")
        {
            Debug.Log("Since the next is global fuzzing, initializing local parameters.");
            
            csvLines.Add($"{cpNum1},{cpNum2},{cpNum3},{cpNum4},{cpNum5},{cpNum6},{cpNum7},{cpNum8},{cpNum9},{cpNum10}," +
                $"{cpNum1_range},{cpNum2_range},{cpNum3_range},{cpNum4_range},{cpNum5_range},{cpNum6_range},{cpNum7_range},{cpNum8_range},{cpNum9_range},{cpNum10_range}," +
                $"{cpNum1_dir},{cpNum2_dir},{cpNum3_dir},{cpNum4_dir},{cpNum5_dir},{cpNum6_dir},{cpNum7_dir},{cpNum8_dir},{cpNum9_dir},{cpNum10_dir}");
        }
        else
        {
            Debug.Log("Since the next is local fuzzing, mutating local parameters.");
            
            csvLines.Add($"{UnityEngine.Random.Range(0, maxNum)},{UnityEngine.Random.Range(0, maxNum)},{UnityEngine.Random.Range(0, maxNum)},{UnityEngine.Random.Range(0, maxNum)},{UnityEngine.Random.Range(0, maxNum)},{UnityEngine.Random.Range(0, maxNum)},{UnityEngine.Random.Range(0, maxNum)},{UnityEngine.Random.Range(0, maxNum)},{UnityEngine.Random.Range(0, maxNum)},{UnityEngine.Random.Range(0, maxNum)}," +
                $"{UnityEngine.Random.Range(0, maxRange)},{UnityEngine.Random.Range(0, maxRange)},{UnityEngine.Random.Range(0, maxRange)},{UnityEngine.Random.Range(0, maxRange)},{UnityEngine.Random.Range(0, maxRange)},{UnityEngine.Random.Range(0, maxRange)},{UnityEngine.Random.Range(0, maxRange)},{UnityEngine.Random.Range(0, maxRange)},{UnityEngine.Random.Range(0, maxRange)},{UnityEngine.Random.Range(0, maxRange)}," +
                $"{UnityEngine.Random.Range(1, 5)},{UnityEngine.Random.Range(1, 5)},{UnityEngine.Random.Range(1, 5)},{UnityEngine.Random.Range(1, 5)},{UnityEngine.Random.Range(1, 5)},{UnityEngine.Random.Range(1, 5)},{UnityEngine.Random.Range(1, 5)},{UnityEngine.Random.Range(1, 5)},{UnityEngine.Random.Range(1, 5)},{UnityEngine.Random.Range(1, 5)}");
        }
        
        File.WriteAllLines(Path.Combine(Application.dataPath, "Logs", "param_local.csv"), csvLines.ToArray());
    }
}
