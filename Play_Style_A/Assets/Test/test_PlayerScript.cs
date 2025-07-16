using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using System;
using System.IO;

public class test_PlayerScript : MonoBehaviour
{
    //private NavMeshAgent agent;
    //private GameObject navDestinationPos = null;
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
    private StreamWriter csvWriter;

    void Start()
    {
        //agent = GetComponent<NavMeshAgent>();
        //agent.destination = this.transform.position;
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

        
        csvWriter = new StreamWriter(csvFilePath);
        csvWriter.WriteLine("Frame,Speed,v,h,CameraForwardX,CameraForwardY,CameraForwardZ,CameraRightX,CameraRightY,CameraRightZ,JumpPressed");
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            mouseCursor();
        }

        
        anim.SetBool("LeftClick", false);

        
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : normalSpeed;

        
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        
        
        

        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        Vector3 moveZ = cameraForward * v * speed;  
        Vector3 moveX = Camera.main.transform.right * h * speed; 

        
        
        if (con.isGrounded)
        {
            moveDirection = moveZ + moveX;
            if (Input.GetButtonDown("Jump"))
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

        
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool("LeftClick", true);
        }

        
        if (Input.GetMouseButtonDown(1))
        {
            anim.SetBool("RightClick", true);
        }
        if (Input.GetMouseButtonUp(1))
        {
            anim.SetBool("RightClick", false);
        }

        
        string jumpPressed = Input.GetButtonDown("Jump") ? "1" : "0";
        csvWriter.WriteLine($"{Time.frameCount},{speed},{v},{h},{cameraForward.x},{cameraForward.y},{cameraForward.z},{Camera.main.transform.right.x},{Camera.main.transform.right.y},{Camera.main.transform.right.z},{jumpPressed}");
    }

    void OnDestroy()
    {
        
        if (csvWriter != null)
        {
            csvWriter.Close();
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
