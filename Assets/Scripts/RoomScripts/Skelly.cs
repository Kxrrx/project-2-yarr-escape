using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skelly : MonoBehaviour
{
    public int chaseRange = 10;
    public int tetherRange;

    private GameObject pirate;
    private PirateController pirateControllerScipt;
    private GameObject currentTarget;
    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 startPos;
    private bool walking = false;

    private float initializationTime;

    void Start()
    {
        initializationTime = Time.timeSinceLevelLoad;

        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        animator = transform.GetChild(0).GetComponent<Animator>();
        pirate = GameObject.FindGameObjectWithTag("Player");
        pirateControllerScipt = pirate.GetComponent<PirateController>();
        InvokeRepeating(nameof(DistanceCheck), 0, 0.5f);
        startPos = transform.position;
    }

    void Update()
    {
        float timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;

        if (timeSinceInitialization > 0.2f)
        {
           agent.enabled = true;
        }
        else
        {
            return;
        }

        if (currentTarget != null)
        {
            agent.destination = pirate.transform.position;

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                walking = true;
            }
            else
            {
                walking = false;
            }
        }
        else
        {
            agent.destination = startPos;
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                walking = true;
            }
            else
            {
                walking = false;
            }
        }
        
        animator.SetBool("walking", walking);
    }

    public void DistanceCheck()
    {
        if(pirateControllerScipt.isDead == true)
        {
            currentTarget = null;
            return;
        }
        else
        {
            float distance = Vector3.Distance(transform.position, pirate.transform.position);

            if (distance < chaseRange)
            {
                currentTarget = pirate;
                agent.destination = pirate.transform.position;
            }
            else if (distance > tetherRange)
            {
                currentTarget = null;
            }
        }      
    }

    //Triggered with animation event in skelly animator:
    public void KillPlayer()
    {
        pirateControllerScipt.isDead = true;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Skelly hits player");
            walking = false;
            animator.SetBool("walking", false);
            animator.Play("Attack");
            agent.destination = agent.transform.position;
        }
    }
}
