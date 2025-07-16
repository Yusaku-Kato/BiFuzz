using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.IO;
using System.Linq;
using NUnit.Framework.Internal.Builders;
using UnityEngine.SceneManagement;
using System;
using UnityEditor.SceneManagement;

public class grad_init : MonoBehaviour
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

    
    [SerializeField]
    public bool ss1;
    [SerializeField]
    public bool ss2;
    [SerializeField]
    public bool ss3;
    [SerializeField]
    public bool ss4;
    [SerializeField]
    public bool ss5;
    [SerializeField]
    public bool ss6;
    [SerializeField]
    public bool ss7;
    [SerializeField]
    public bool ss8;
    [SerializeField]
    public bool ss9;
    [SerializeField]
    public bool ss10;
    [SerializeField, Range(0, 9)]
    public int ss1_pri;
    [SerializeField, Range(0, 9)]
    public int ss2_pri;
    [SerializeField, Range(0, 9)]
    public int ss3_pri;
    [SerializeField, Range(0, 9)]
    public int ss4_pri;
    [SerializeField, Range(0, 9)]
    public int ss5_pri;
    [SerializeField, Range(0, 9)]
    public int ss6_pri;
    [SerializeField, Range(0, 9)]
    public int ss7_pri;
    [SerializeField, Range(0, 9)]
    public int ss8_pri;
    [SerializeField, Range(0, 9)]
    public int ss9_pri;
    [SerializeField, Range(0, 9)]
    public int ss10_pri;

    
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

    
    public struct ss
    {
        public int index;
        public bool on;
        public int pri;
        public Vector3 loc;

        
        public ss(int index, bool on, int pri, Vector3 loc)
        {
            this.index = index;
            this.on = on;
            this.pri = pri;
            this.loc = loc;
        }
    }
    ss[] ss_info;
    ss[] strategy;

    
    Vector3 postPos = Vector3.zero;
    Vector3 nowPos = Vector3.zero;
    Vector3 fixed_nowPos = Vector3.zero;
    int avdCnt = 0;
    bool isAvding = false;
    bool isAvoidRight = false;

    
    string filePath = Path.Combine(Application.dataPath, "Logs", "init_fuzz.csv");

    void Start()
    {
        
        try
        {
            
            using (StreamWriter writer = new StreamWriter(Path.Combine(Application.dataPath, "Logs", "detected_bugs.csv"), true))
            {
                writer.WriteLine("init");
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to create CSV file: {e.Message}");
        }

        
        if (!File.Exists(Path.Combine(Application.dataPath, "Logs", "param_global.csv")))
        {
            try
            {
                
                using (StreamWriter writer = new StreamWriter(Path.Combine(Application.dataPath, "Logs", "param_global.csv"), false))
                {
                    
                    writer.WriteLine("ss1,ss2,ss3,ss4,ss5,ss6,ss7,ss8,ss9,ss10,ss1_pri,ss2_pri,ss3_pri,ss4_pri,ss5_pri,ss6_pri,ss7_pri,ss8_pri,ss9_pri,ss10_pri");
                    writer.WriteLine($"{ss1},{ss2},{ss3},{ss4},{ss5},{ss6},{ss7},{ss8},{ss9},{ss10},{ss1_pri},{ss2_pri},{ss3_pri},{ss4_pri},{ss5_pri},{ss6_pri},{ss7_pri},{ss8_pri},{ss9_pri},{ss10_pri}");
                    Debug.Log($"Parameters recorded: param_global.csv");
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to create CSV file: {e.Message}");
            }
        }

        
        List<string> paramLines = new List<string>(File.ReadAllLines(Path.Combine(Application.dataPath, "Logs", "param_global.csv")));
        string paramLastLine = paramLines[paramLines.Count - 1];
        string[] paramColumns = paramLastLine.Split(',');
        Debug.Log("ss1: " + bool.Parse(paramColumns[0]));
        Debug.Log("ss10_pri: " + int.Parse(paramColumns[19]));

        
        ss_info = new ss[10];
        ss_info[0] = new ss(1, bool.Parse(paramColumns[0]), int.Parse(paramColumns[10]), ss1_loc);
        ss_info[1] = new ss(2, bool.Parse(paramColumns[1]), int.Parse(paramColumns[11]), ss2_loc);
        ss_info[2] = new ss(3, bool.Parse(paramColumns[2]), int.Parse(paramColumns[12]), ss3_loc);
        ss_info[3] = new ss(4, bool.Parse(paramColumns[3]), int.Parse(paramColumns[13]), ss4_loc);
        ss_info[4] = new ss(5, bool.Parse(paramColumns[4]), int.Parse(paramColumns[14]), ss5_loc);
        ss_info[5] = new ss(6, bool.Parse(paramColumns[5]), int.Parse(paramColumns[15]), ss6_loc);
        ss_info[6] = new ss(7, bool.Parse(paramColumns[6]), int.Parse(paramColumns[16]), ss7_loc);
        ss_info[7] = new ss(8, bool.Parse(paramColumns[7]), int.Parse(paramColumns[17]), ss8_loc);
        ss_info[8] = new ss(9, bool.Parse(paramColumns[8]), int.Parse(paramColumns[18]), ss9_loc);
        ss_info[9] = new ss(10, bool.Parse(paramColumns[9]), int.Parse(paramColumns[19]), ss10_loc);

        
        System.Random random = new System.Random();
        strategy = ss_info.Where(i => i.on).OrderByDescending(i => i.pri).ThenBy(i => random.Next()).ToArray();

        
        try
        {
            
            using (StreamWriter writer = new StreamWriter(Path.Combine(Application.dataPath, "Logs", "check_points.csv"), false))
            {
                writer.WriteLine("index,x,y,z"); 
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to create CSV file: {e.Message}");
        }

        
        Debug.Log("=== Sorted array ===");
        foreach (var i in strategy)
        {
            Debug.Log($"Index: {i.index}, Priority: {i.pri}, Location: {i.loc}");
            
            try
            {
                
                using (StreamWriter writer = new StreamWriter(Path.Combine(Application.dataPath, "Logs", "check_points.csv"), true))
                {
                    writer.WriteLine($"{i.index},{i.loc.x},{i.loc.y},{i.loc.z}");
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to create CSV file: {e.Message}");
            }
        }

        
        GameObject stuckBugsObject = GameObject.Find("StuckBugs");
        if (stuckBugsObject != null)
        {
            stuckBugsObject.SetActive(false);
            Debug.Log("StuckBugs object deactivated.");
        }
        else
        {
            Debug.LogError("StuckBugs object not found.");
        }

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
                
                writer.WriteLine("speed,v,h,forwardX,forwardZ,jump,item");
                Debug.Log($"Created CSV file 'init_fuzz.csv': {filePath}");
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

        
        if (frameIndex == 72000)
        {
            ss1_pri++;
            Debug.Log("Timeout");
            
            if (File.Exists(Path.Combine(Application.dataPath, "Logs", "fuzz_type.csv")))
            {
                using (StreamWriter writer = new StreamWriter(Path.Combine(Application.dataPath, "Logs", "fuzz_type.csv"), true))
                {
                    writer.WriteLine("init");
                }
                Debug.Log("Wrote to fuzz_type.csv file.");
            }
            else
            {
                Debug.Log("Error: fuzz_type.csv file does not exist.");
                UnityEditor.EditorApplication.isPlaying = false;
            }
            MutateParam();
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.path); 
            return;
        }

        
        //if (nowPos.x == strategy[0].loc.x && nowPos.z == strategy[0].loc.z)
        //{
        //    strategy = strategy.Skip(1).ToArray();
        //}
        
        if (Mathf.Abs(nowPos.x - strategy[0].loc.x) <= 0.1f && Mathf.Abs(nowPos.z - strategy[0].loc.z) <= 0.1f)
        {
            
            List<string> lines = new List<string>(File.ReadAllLines(filePath));
            int lastLine = lines.Count - 1;
            
            lines[lastLine] += strategy[0].index.ToString();
            
            File.WriteAllLines(filePath, lines.ToArray());

            strategy = strategy.Skip(1).ToArray();

            if (strategy.Length == 0)
            {
                ss1_pri++;
                Debug.Log("Initial input generation complete");
                
                if (File.Exists(Path.Combine(Application.dataPath, "Logs", "fuzz_type.csv")))
                {
                    using (StreamWriter writer = new StreamWriter(Path.Combine(Application.dataPath, "Logs", "fuzz_type.csv"), true))
                    {
                        writer.WriteLine("local");
                    }
                    Debug.Log("Wrote to fuzz_type.csv file.");
                }
                else
                {
                    Debug.Log("Error: fuzz_type.csv file does not exist.");
                    UnityEditor.EditorApplication.isPlaying = false;
                }
                MutateParam();
                Scene scene = SceneManager.GetActiveScene(); 
                SceneManager.LoadScene(scene.path); 
                return;
            }
            Debug.Log("Item collected");
        }

        
        if (frameIndex % 60 == 0)
        {
            float distance = Vector3.Distance(new Vector3(postPos.x, 0, postPos.z), new Vector3(nowPos.x, 0, nowPos.z));
            if(distance < 1f)
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
            if(avdCnt == 0) 
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
            else if(avdCnt == 60) 
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
                string newLine = $"10,1,0,{direction.x},{direction.z},0,";
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

    
    private void MutateParam()
    {
        int minNum = 10; 
        int maxNum = 11; 
        
        bool[] boolArray = new bool[10];
        
        for (int i = 0; i < boolArray.Length; i++)
        {
            boolArray[i] = false;
        }

        
        int trueCount = 0;
        while (trueCount < UnityEngine.Random.Range(minNum, maxNum))
        {
            int randomIndex = UnityEngine.Random.Range(0, boolArray.Length);
            if (!boolArray[randomIndex])  
            {
                boolArray[randomIndex] = true;
                trueCount++;
            }
        }

        List<string> csvLines = new List<string>(File.ReadAllLines(Path.Combine(Application.dataPath, "Logs", "param_global.csv")));
        Debug.Log("Since the next is local fuzzing, mutating local parameters.");
        csvLines.Add($"{boolArray[0]},{boolArray[1]},{boolArray[2]},{boolArray[3]},{boolArray[4]},{boolArray[5]},{boolArray[6]},{boolArray[7]},{boolArray[8]},{boolArray[9]}," +
            $"{UnityEngine.Random.Range(0, 10)},{UnityEngine.Random.Range(0, 10)},{UnityEngine.Random.Range(0, 10)},{UnityEngine.Random.Range(0, 10)},{UnityEngine.Random.Range(0, 10)},{UnityEngine.Random.Range(0, 10)},{UnityEngine.Random.Range(0, 10)},{UnityEngine.Random.Range(0, 10)},{UnityEngine.Random.Range(0, 10)},{UnityEngine.Random.Range(0, 10)}");
        
        File.WriteAllLines(Path.Combine(Application.dataPath, "Logs", "param_global.csv"), csvLines.ToArray());
    }

    
    //private void MutateParam()
    //{
    
    //    float trueProb = 0.8f;
    
    //    bool[] boolArray = new bool[10];
    
    //    for (int i = 0; i < boolArray.Length; i++)
    //    {
    //        boolArray[i] = UnityEngine.Random.value < trueProb;
    //    }

    //    int Random0to9(int input)
    //    {
    
    //        if (input < 0 || input > 9)
    //        {
    //            throw new ArgumentOutOfRangeException("input", "Input must be between 0 and 9");
    //        }

    
    //        int centerValue = 9 - input;
    //        System.Collections.Generic.List<int> weightedList = new System.Collections.Generic.List<int>();

    //        for (int i = 0; i < 10; i++)
    //        {
    
    //            for (int j = 0; j < weight; j++)
    //            {
    //                weightedList.Add(i);
    //            }
    //        }
    //        //Debug.Log("All Numbers: " + string.Join(", ", weightedList));
    
    //        int randomIndex = UnityEngine.Random.Range(0, weightedList.Count);
    //        return weightedList[randomIndex];
    //    }
    //    List<string> csvLines = new List<string>(File.ReadAllLines(Path.Combine(Application.dataPath, "Logs", "param_global.csv")));
    
    //    csvLines.Add($"{boolArray[0]},{boolArray[1]},{boolArray[2]},{boolArray[3]},{boolArray[4]},{boolArray[5]},{boolArray[6]},{boolArray[7]},{boolArray[8]},{boolArray[9]}," +
    //        $"{Random0to9(0)},{Random0to9(1)},{Random0to9(2)},{Random0to9(3)},{Random0to9(4)},{Random0to9(5)},{Random0to9(6)},{Random0to9(7)},{Random0to9(8)},{Random0to9(9)}");
    
    //    File.WriteAllLines(Path.Combine(Application.dataPath, "Logs", "param_global.csv"), csvLines.ToArray());
    //}
}
