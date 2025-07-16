using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoController3 : MonoBehaviour
{
    PlayerScript playerscript;
    int count;
    float prevRandomFloat1;
    float prevRandomFloat2;

    private NavMeshTriangle navMeshTriangle;
    double prevCoverage = 0;
    double nowCoverage = 0;
    // Start is called before the first frame update
    void Start()
    {
        playerscript = GetComponent<PlayerScript>();
        count = 0;

        navMeshTriangle = GetComponent<NavMeshTriangle>();
        nowCoverage = navMeshTriangle.triangleCoverage;
    }

    // Update is called once per frame
    void Update()
    {
        if (count % 60 == 0)
        {
            nowCoverage = navMeshTriangle.triangleCoverage;
            if (prevCoverage != nowCoverage)
            {
                System.Random random = new System.Random();
                float zeroOne1 = random.Next(0, 2);
                float zeroOne2 = random.Next(0, 2);
                float randomFloat1 = zeroOne1 == 0 ? 1 : -1;
                float randomFloat2 = zeroOne2 == 0 ? 1 : -1;
                Debug.Log("v = " + randomFloat1);
                Debug.Log("h = " + randomFloat2);
                playerscript.v = randomFloat1;
                playerscript.h = randomFloat2;
                prevCoverage = nowCoverage;
            }
            else
            {
                prevCoverage = nowCoverage;
            }
        }
        count++;
    }

    bool Judge(float x, float y)
    {
        if ((x < 0 && y > 0) || (x > 0 && y < 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}