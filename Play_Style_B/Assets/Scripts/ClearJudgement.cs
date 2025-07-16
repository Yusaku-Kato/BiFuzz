using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

public class ClearJudgement : MonoBehaviour
{
    public int sheepNum;
    public int goatNum;
    public int itemNum;
    private GameObject gameClear;

    // Start is called before the first frame update
    void Start()
    {
        
        sheepNum = GameObject.FindGameObjectsWithTag("Evasive").Length;
        goatNum = GameObject.FindGameObjectsWithTag("Enemy").Length;
        itemNum = GameObject.FindGameObjectsWithTag("Item").Length;

        gameClear = GameObject.Find("GameClear");
        gameClear.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (sheepNum - GameObject.FindGameObjectsWithTag("Evasive").Length >= 10 && goatNum - GameObject.FindGameObjectsWithTag("Enemy").Length >= 10 && itemNum - GameObject.FindGameObjectsWithTag("Item").Length >= 10)
        {
            gameClear.gameObject.SetActive(true);
        }
    }
}
