using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestMovingSphere : MonoBehaviour
{
    private NavMeshAgent agent;
    //public Transform target;
    private GameObject navDestinationPos = null; //

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.destination = target.position;
        agent.destination = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        navDestinationPos = null;
        navDestinationPos = GameObject.Find("NavDestination(Clone)"); //
        
        if(navDestinationPos != null)
        {
            agent.destination = navDestinationPos.transform.position;
        }
        else
        {
            agent.destination = this.transform.position;
        }
        //agent.destination = target.position;
    }
}
