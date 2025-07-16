using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoController : MonoBehaviour
{
    PlayerScript playerscript;
    int count;
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
            Debug.Log("v = " + randomFloat1);
            Debug.Log("h = " + randomFloat2);
            playerscript.v = randomFloat1;
            playerscript.h = randomFloat2;
        }
        count++;
    }
}
