using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoController2 : MonoBehaviour
{
    PlayerScript playerscript;
    int count;
    float prevRandomFloat1;
    float prevRandomFloat2;
    // Start is called before the first frame update
    void Start()
    {
        playerscript = GetComponent<PlayerScript>();
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (count % 60 == 0)
        {
            System.Random random = new System.Random();
            float zeroOne1 = random.Next(0, 2);
            float zeroOne2 = random.Next(0, 2);
            float randomFloat1 = zeroOne1 == 0 ? 1 : -1;
            float randomFloat2 = zeroOne2 == 0 ? 1 : -1;
            if (Judge(randomFloat1, prevRandomFloat1) && Judge(randomFloat2, prevRandomFloat2)) {
                float zeroOne3 = random.Next(0, 2);
                if(zeroOne3 == 0)
                {
                    randomFloat1 = -randomFloat1;
                }
                else
                {
                    randomFloat2 = -randomFloat2;
                }
            }
            Debug.Log("v = " + randomFloat1);
            Debug.Log("h = " + randomFloat2);
            playerscript.v = randomFloat1;
            playerscript.h = randomFloat2;
            prevRandomFloat1 = randomFloat1;
            prevRandomFloat2 = randomFloat2;
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