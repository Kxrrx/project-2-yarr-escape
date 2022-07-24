using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    public enum TriggerType
    {
        moveTrigger, trapTrigger, rotateTrigger
    }
    public TriggerType triggerType;
    public Animator animator;
    public bool isSingleUse = false;
    public bool currentTriggerState = false;

    public GameObject objectOnTheTrigger; 
    public int[] ids;

    private bool isColliding;
    
    private void Start()
    {
        if (!currentTriggerState)
        {
            animator.SetBool("Triggered", false);
        }
        else
        {
            animator.SetBool("Triggered", true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(!currentTriggerState) 
        {    
            if (other.tag == "Barrel" || other.tag == "Player" || other.tag == "Skelly")
            {
                objectOnTheTrigger = other.gameObject; 

                Debug.Log("On stay");

                currentTriggerState = true;
                animator.SetBool("Triggered", true);

                if (triggerType == TriggerType.moveTrigger)
                {
                    foreach (int objectId in ids)
                    {
                        GameEvents.current.DoorwayTriggerEnter(objectId);

                    }
                }
                if (triggerType == TriggerType.trapTrigger)
                {
                    foreach (int objectId in ids)
                    {
                        GameEvents.current.TrapTriggerEnter(objectId);
                    }
                }
            }
        }      
    }
    private void OnTriggerExit(Collider other)
    {
        if (currentTriggerState && !isSingleUse)
        {
            if (other.gameObject == objectOnTheTrigger)
            {
                objectOnTheTrigger = null;
                Debug.Log("OnTriggerExit");

                currentTriggerState = false;
                animator.SetBool("Triggered", false);

                if (triggerType == TriggerType.moveTrigger)
                {
                    foreach (int objectId in ids)
                    {
                        GameEvents.current.DoorwayTriggerExit(objectId);
                    }
                }
                if (triggerType == TriggerType.trapTrigger)
                {
                    foreach (int objectId in ids)
                    {
                        GameEvents.current.TrapTriggerExit(objectId);
                    }
                }
            }          
        }
    }
}
