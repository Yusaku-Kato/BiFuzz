using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class TestSheep : MonoBehaviour
{
    private Animator animator;              
    private NavMeshAgent agent;             
    public Transform target;                
    private Rigidbody rb;

    Vector3 newPosition;                    
    Vector3 evadeDestination;               
    int hitPoint = 10;                      

    
    public GameObject targetObject;
    
    private PlayerScript playerScript;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();    
        agent = GetComponent<NavMeshAgent>();   
        
        rb = GetComponent<Rigidbody>();
        newPosition = this.transform.position;  
        evadeDestination = this.transform.position;

        
        playerScript = targetObject.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(hitPoint <= 0)
        {
            animator.SetTrigger("Getdeath");
            GameObject.Destroy(this);
        }

        Vector3 prePosition = newPosition;      
        newPosition = this.transform.position;  

        Vector3 positionDiff = target.position - newPosition;
        //if (positionDiff.magnitude <= 10)
        //{
        //    evadeDestination.x = this.transform.position.x + (this.transform.position.x - target.position.x);
        //    evadeDestination.z = this.transform.position.z + (this.transform.position.z - target.position.z);
        //    evadeDestination.y = this.transform.position.y;
        
        //}
        //else
        //{
        //    evadeDestination.x = this.transform.position.x + Random.Range(-5.0f, 5.0f);
        //    evadeDestination.z = this.transform.position.z + Random.Range(-5.0f, 5.0f);
        //    evadeDestination.y = this.transform.position.y + Random.Range(-5.0f, 5.0f);
        
        //}

        
        //animator.SetFloat("MoveSpeed", (evadeDestination - this.transform.position).magnitude);
        animator.SetFloat("MoveSpeed", (newPosition - prePosition).magnitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        Damager damager = other.GetComponent<Damager>();
        if (damager != null)
        {
            
            if (playerScript.isAttacking == true)
            {
                
                animator.SetTrigger("Gethit");       
                hitPoint = hitPoint - damager.damage;
            }
        }
    }
}