using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using System;
using System.IO;

public class test_PlayerScript_Replay : MonoBehaviour
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

    
    private string csvFilePath;
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

        
        csvFilePath = Path.Combine(Application.dataPath, "Logs", "player_data.csv");

        
        string directoryPath = Path.GetDirectoryName(csvFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        
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

        currentFrameIndex = 0;
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
