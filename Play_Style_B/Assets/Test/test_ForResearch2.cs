using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class ScoringSystem : MonoBehaviour
{
    public int autoTestingScore = 0;
    int count = 0;
    Vector3 vector3SoftStar1;

    private void Start()
    {
        
        vector3SoftStar1 = GameObject.Find("SoftStar 1").transform.position;
    }

    void Update()
    {
        if(Vector3.Distance(this.transform.position, vector3SoftStar1) <= 1f)
        {
            autoTestingScore++;
        }

        if(count % 60 == 0)
        {
            Debug.Log("The score is " + autoTestingScore + " now.");
        }
        count++;
    }
}
