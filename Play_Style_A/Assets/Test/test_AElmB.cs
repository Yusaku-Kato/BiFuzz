using UnityEngine;

public class DisableCollisionWithPlayer : MonoBehaviour
{
    
    public Collider objectBCollider;

    
    private bool isPlayerColliding = false;

    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            isPlayerColliding = true;
            objectBCollider.enabled = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            isPlayerColliding = false;
            objectBCollider.enabled = true;
        }
    }

    void Update()
    {
        
        //Debug.Log("Is Player Colliding: " + isPlayerColliding);
    }
}
