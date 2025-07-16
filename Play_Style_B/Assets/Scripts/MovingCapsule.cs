using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Apple;

public class MovingCapsule : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 destination;
    private int count = 20;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        destination = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (count == 20)
        {
            destination.x = this.transform.position.x + Random.Range(-100.0f, 100.0f);
            destination.z = this.transform.position.z + Random.Range(-100.0f, 100.0f);
            destination.y = this.transform.position.y + Random.Range(-100.0f, 100.0f);
            agent.destination = destination;   //ƒ‰ƒ“ƒ_ƒ€‚ÅˆÚ“®
            count = 0;
        }
        else
        {
            count++;
        }
    }
}
