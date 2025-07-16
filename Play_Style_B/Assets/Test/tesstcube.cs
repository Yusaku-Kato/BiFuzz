using UnityEngine;
using UnityEngine.AI;

public class CheckNavMesh : MonoBehaviour
{
    void Update()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            Debug.Log("The attached object is on the NavMesh.");
        }
        else
        {
            Debug.Log("The attached object is not on the NavMesh.");
        }
    }
}
