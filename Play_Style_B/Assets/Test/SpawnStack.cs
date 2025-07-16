using UnityEngine;

public class SpawnStack : MonoBehaviour
{
    public GameObject stackPrefab; 
    private bool hasSpawned = false; 
    public Vector3 playerPosition;

    void Update()
    {
        if (hasSpawned)
        {
            return; 
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerPosition = player.transform.position;
            if (IsWithinRange(playerPosition.x, 4f, 6f) && IsWithinRange(playerPosition.z, 4f, 6f))
            {
                Instantiate(stackPrefab, playerPosition, Quaternion.identity);
                hasSpawned = true; 
            }
        }
    }

    private bool IsWithinRange(float value, float min, float max)
    {
        return value >= min && value <= max;
    }
}
