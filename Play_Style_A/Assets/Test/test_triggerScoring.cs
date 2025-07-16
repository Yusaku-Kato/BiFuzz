using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class test_triggerScoring : MonoBehaviour
{
    ScoringSystem scoringSystem;
    int i;

    // Start is called before the first frame update
    void Start()
    {
        scoringSystem = GetComponent<ScoringSystem>();
        i = scoringSystem.autoTestingScore;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        //scoringSystem.autoTestingScore++;
        i++;
    }
}
