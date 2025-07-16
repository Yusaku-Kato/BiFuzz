using UnityEngine;

public class AddRigidbodyOnTrigger : MonoBehaviour
{
    
    public GameObject objectB;

    
    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        
        if (!hasTriggered && other.CompareTag("Player"))
        {
            
            hasTriggered = true;

            
            if (objectB != null)
            {
                if (objectB.GetComponent<Rigidbody>() == null)
                {
                    objectB.AddComponent<Rigidbody>();
                }
                else
                {
                    Debug.LogWarning("objectB already has a Rigidbody component.");
                }
            }
            else
            {
                Debug.LogError("objectB is not assigned.");
            }
        }
    }
}
