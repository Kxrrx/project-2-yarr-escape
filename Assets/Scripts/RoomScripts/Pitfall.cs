using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitfall : MonoBehaviour
{
    private PirateController pirateControllerScript;
    void Start()
    {
        pirateControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PirateController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            pirateControllerScript.animator.Play("Fall");
            Camera.main.GetComponent<CameraFollow>().enabled = false;
        }
    }      
}
