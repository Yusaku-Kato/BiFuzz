using UnityEngine;

public class DisableCollisionOnPlayerContact : MonoBehaviour
{
    
    public GameObject[] targetObjects = new GameObject[4];

    
    private bool hasPlayerContacted = false;

    
    //void OnCollisionEnter(Collision collision)
    //{
    
    //    if (!hasPlayerContacted && collision.gameObject.tag == "Player")
    //    {
    //        hasPlayerContacted = true;

    
    //        foreach (GameObject obj in targetObjects)
    //        {
    //            Collider objCollider = obj.GetComponent<Collider>();
    //            objCollider.enabled = false;
    //        }
    //    }
    //}

    void OnTriggerEnter(Collider other)
    {
        
        if (!hasPlayerContacted && other.gameObject.tag == "Player")
        {
            hasPlayerContacted = true;

            
            foreach (GameObject obj in targetObjects)
            {
                Collider objCollider = obj.GetComponent<Collider>();
                objCollider.enabled = false;
            }
        }
    }
}