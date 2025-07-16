using UnityEngine;

public class Item : MonoBehaviour
{
    // This method is called when another collider enters the trigger collider attached to the object this script is attached to.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other collider has the tag "Player"
        if (other.CompareTag("Player"))
        {
            // Destroy this game object
            Destroy(gameObject);
        }
    }
}
