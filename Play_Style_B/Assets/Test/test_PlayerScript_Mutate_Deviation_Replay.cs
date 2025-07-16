using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.IO;
using System;

public class test_PlayerScript_Mutate_Deviation_Replay : MonoBehaviour
{
    CharacterController con;
    Animator anim;
    CinemachineFreeLook freeLook;
    private bool isPushedM = false;
    public GameObject freeLookCamera;
    float normalSpeed = 6f;
    float sprintSpeed = 10f;
    float jump = 16f;
    float gravity = 50f;
    Vector3 moveDirection = Vector3.zero;
    public bool isAttacking = false;
    Vector3 startPos;
    public GameObject targetObject;
    public float v;
    public float h;
    private string originalCsvFilePath;
    private string mutatedCsvFilePath;
    private List<string[]> csvData;
    private int currentFrameIndex;

    void Start()
    {
        con = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        freeLookCamera = GameObject.Find("FreeLook Camera");
        freeLook = freeLookCamera.GetComponent<CinemachineFreeLook>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        startPos = transform.position;
        originalCsvFilePath = Path.Combine(Application.dataPath, "Logs", "player_data.csv");
        mutatedCsvFilePath = Path.Combine(Application.dataPath, "Logs", "mutated_player_data.csv");
        csvData = new List<string[]>();

        if (File.Exists(originalCsvFilePath))
        {
            using (StreamReader sr = new StreamReader(originalCsvFilePath))
            {
                string line;
                sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    csvData.Add(line.Split(','));
                }
            }
            MutateData();
        }
        else
        {
            Debug.LogError("CSV file not found: " + originalCsvFilePath);
        }

        currentFrameIndex = 0;
    }

    void MutateData()
    {
        float moveZ_x_mean = 0f, moveZ_z_mean = 0f, moveX_x_mean = 0f, moveX_z_mean = 0f;
        float moveZ_x_std = 0f, moveZ_z_std = 0f, moveX_x_std = 0f, moveX_z_std = 0f;

        List<float> moveZ_x_list = new List<float>();
        List<float> moveZ_z_list = new List<float>();
        List<float> moveX_x_list = new List<float>();
        List<float> moveX_z_list = new List<float>();

        foreach (var row in csvData)
        {
            float speed = float.Parse(row[1]);
            float v = float.Parse(row[2]);
            float h = float.Parse(row[3]);
            Vector3 cameraForward = new Vector3(float.Parse(row[4]), float.Parse(row[5]), float.Parse(row[6]));
            Vector3 cameraRight = new Vector3(float.Parse(row[7]), float.Parse(row[8]), float.Parse(row[9]));

            Vector3 moveZ = cameraForward * v * speed;
            Vector3 moveX = cameraRight * h * speed;

            moveZ_x_list.Add(moveZ.x);
            moveZ_z_list.Add(moveZ.z);
            moveX_x_list.Add(moveX.x);
            moveX_z_list.Add(moveX.z);
        }

        moveZ_x_mean = Mean(moveZ_x_list);
        moveZ_z_mean = Mean(moveZ_z_list);
        moveX_x_mean = Mean(moveX_x_list);
        moveX_z_mean = Mean(moveX_z_list);

        moveZ_x_std = StandardDeviation(moveZ_x_list, moveZ_x_mean);
        moveZ_z_std = StandardDeviation(moveZ_z_list, moveZ_z_mean);
        moveX_x_std = StandardDeviation(moveX_x_list, moveX_x_mean);
        moveX_z_std = StandardDeviation(moveX_z_list, moveX_z_mean);

        float v_min = float.MaxValue;
        float v_max = float.MinValue;
        float h_min = float.MaxValue;
        float h_max = float.MinValue;
        float cameraForwardX_min = float.MaxValue;
        float cameraForwardX_max = float.MinValue;
        float cameraForwardZ_min = float.MaxValue;
        float cameraForwardZ_max = float.MinValue;
        float cameraRightX_min = float.MaxValue;
        float cameraRightX_max = float.MinValue;
        float cameraRightZ_min = float.MaxValue;
        float cameraRightZ_max = float.MinValue;

        foreach (var row in csvData)
        {
            v_min = Mathf.Min(v_min, float.Parse(row[2]));
            v_max = Mathf.Max(v_max, float.Parse(row[2]));
            h_min = Mathf.Min(h_min, float.Parse(row[3]));
            h_max = Mathf.Max(h_max, float.Parse(row[3]));
            cameraForwardX_min = Mathf.Min(cameraForwardX_min, float.Parse(row[4]));
            cameraForwardX_max = Mathf.Max(cameraForwardX_max, float.Parse(row[4]));
            cameraForwardZ_min = Mathf.Min(cameraForwardZ_min, float.Parse(row[6]));
            cameraForwardZ_max = Mathf.Max(cameraForwardZ_max, float.Parse(row[6]));
            cameraRightX_min = Mathf.Min(cameraRightX_min, float.Parse(row[7]));
            cameraRightX_max = Mathf.Max(cameraRightX_max, float.Parse(row[7]));
            cameraRightZ_min = Mathf.Min(cameraRightZ_min, float.Parse(row[9]));
            cameraRightZ_max = Mathf.Max(cameraRightZ_max, float.Parse(row[9]));
        }

        for (int i = 0; i < csvData.Count; i++)
        {
            float speed = float.Parse(csvData[i][1]);
            float v = float.Parse(csvData[i][2]);
            float h = float.Parse(csvData[i][3]);
            Vector3 cameraForward = new Vector3(float.Parse(csvData[i][4]), float.Parse(csvData[i][5]), float.Parse(csvData[i][6]));
            Vector3 cameraRight = new Vector3(float.Parse(csvData[i][7]), float.Parse(csvData[i][8]), float.Parse(csvData[i][9]));

            Vector3 moveZ = cameraForward * v * speed;
            Vector3 moveX = cameraRight * h * speed;

            float moveZ_x_score = StandardScore(moveZ.x, moveZ_x_mean, moveZ_x_std);
            float moveZ_z_score = StandardScore(moveZ.z, moveZ_z_mean, moveZ_z_std);
            float moveX_x_score = StandardScore(moveX.x, moveX_x_mean, moveX_x_std);
            float moveX_z_score = StandardScore(moveX.z, moveX_z_mean, moveX_z_std);

            if (Mathf.Abs(moveZ_x_score - 50) > 2 * moveZ_x_std || Mathf.Abs(moveZ_z_score - 50) > 2 * moveZ_z_std ||
                Mathf.Abs(moveX_x_score - 50) > 2 * moveX_x_std || Mathf.Abs(moveX_z_score - 50) > 2 * moveX_z_std)
            {
                for (int j = i; j < i + 60 && j < csvData.Count; j++)
                {
                    if (j == i)
                    {
                        csvData[j][2] = UnityEngine.Random.Range(v_min, v_max).ToString();
                        csvData[j][3] = UnityEngine.Random.Range(h_min, h_max).ToString();
                        csvData[j][4] = UnityEngine.Random.Range(cameraForwardX_min, cameraForwardX_max).ToString();
                        csvData[j][6] = UnityEngine.Random.Range(cameraForwardZ_min, cameraForwardZ_max).ToString();
                        csvData[j][7] = UnityEngine.Random.Range(cameraRightX_min, cameraRightX_max).ToString();
                        csvData[j][9] = UnityEngine.Random.Range(cameraRightZ_min, cameraRightZ_max).ToString();
                    }
                    else
                    {
                        float v_diff = float.Parse(csvData[j][2]) - float.Parse(csvData[i][2]);
                        float h_diff = float.Parse(csvData[j][3]) - float.Parse(csvData[i][3]);
                        float cameraForwardX_diff = float.Parse(csvData[j][4]) - float.Parse(csvData[i][4]);
                        float cameraForwardZ_diff = float.Parse(csvData[j][6]) - float.Parse(csvData[i][6]);
                        float cameraRightX_diff = float.Parse(csvData[j][7]) - float.Parse(csvData[i][7]);
                        float cameraRightZ_diff = float.Parse(csvData[j][9]) - float.Parse(csvData[i][9]);

                        csvData[j][2] = (float.Parse(csvData[i][2]) + v_diff).ToString();
                        csvData[j][3] = (float.Parse(csvData[i][3]) + h_diff).ToString();
                        csvData[j][4] = (float.Parse(csvData[i][4]) + cameraForwardX_diff).ToString();
                        csvData[j][6] = (float.Parse(csvData[i][6]) + cameraForwardZ_diff).ToString();
                        csvData[j][7] = (float.Parse(csvData[i][7]) + cameraRightX_diff).ToString();
                        csvData[j][9] = (float.Parse(csvData[i][9]) + cameraRightZ_diff).ToString();
                    }
                }
            }
        }

        using (StreamWriter sw = new StreamWriter(mutatedCsvFilePath))
        {
            sw.WriteLine("Frame,Speed,v,h,CameraForwardX,CameraForwardY,CameraForwardZ,CameraRightX,CameraRightY,CameraRightZ,JumpPressed");
            foreach (var row in csvData)
            {
                sw.WriteLine(string.Join(",", row));
            }
        }
    }

    float Mean(List<float> values)
    {
        float sum = 0f;
        foreach (float val in values)
        {
            sum += val;
        }
        return sum / values.Count;
    }

    float StandardDeviation(List<float> values, float mean)
    {
        float sumOfSquares = 0f;
        foreach (float val in values)
        {
            sumOfSquares += Mathf.Pow(val - mean, 2);
        }
        return Mathf.Sqrt(sumOfSquares / values.Count);
    }

    float StandardScore(float value, float mean, float std)
    {
        return 50 + 10 * ((value - mean) / std);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            mouseCursor();
        }

        anim.SetBool("LeftClick", false);

        if (csvData != null && currentFrameIndex < csvData.Count)
        {
            var frameData = csvData[currentFrameIndex];
            float speed = float.Parse(frameData[1]);
            v = float.Parse(frameData[2]);
            h = float.Parse(frameData[3]);
            Vector3 cameraForward = new Vector3(float.Parse(frameData[4]), float.Parse(frameData[5]), float.Parse(frameData[6]));
            Vector3 cameraRight = new Vector3(float.Parse(frameData[7]), float.Parse(frameData[8]), float.Parse(frameData[9]));
            bool jumpPressed = frameData[10] == "1";

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

            currentFrameIndex++;
        }
        else if (currentFrameIndex >= csvData.Count)
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
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

    private void mouseCursor()
    {
        if (isPushedM)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            freeLook.m_XAxis.m_MaxSpeed = 900;
            freeLook.m_YAxis.m_MaxSpeed = 6;
            isPushedM = false;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            freeLook.m_XAxis.m_MaxSpeed = 0;
            freeLook.m_YAxis.m_MaxSpeed = 0;
            isPushedM = true;
        }
    }
}
