using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
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
            pirateControllerScript.nearestPickupItem = gameObject;
            LevelManager.singleton.hintText.text = "Hold [space] to carry the barrel.";
            LevelManager.singleton.hintText.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            pirateControllerScript.nearestPickupItem = null;
            LevelManager.singleton.hintText.text = "";
            LevelManager.singleton.hintText.gameObject.SetActive(false);
        }
    }
}
