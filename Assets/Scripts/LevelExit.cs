using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private LevelManager levelManager;
    private Vector3 victoryPosition = new Vector3(0, 0.1f, 16.75f);
    private GameObject pirate;
    private PirateController pirateControllerScript;
    private Animator pirateAnimator;

    private AudioSource[] allAudioSources;

    private void Awake()
    {
        allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
    }
    private void Start()
    {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        pirate = GameObject.FindGameObjectWithTag("Player");
        pirateControllerScript = pirate.GetComponent<PirateController>();
        pirateAnimator = pirateControllerScript.animator;
        if (GameObject.Find("PlayerVictoryPosition") != null)
        {
            victoryPosition = GameObject.Find("PlayerVictoryPosition").transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        if (levelManager.currentRoomIndex == levelManager.roomScenes.Count - 1)
        {
            Debug.Log("You escaped succesfully!");
            foreach (AudioSource audioS in allAudioSources)
            {
                audioS.Stop();
            }
            AudioManager.singleton.Stop("Gameplay");
            AudioManager.singleton.Play("Victory");
            pirate.GetComponent<PirateController>().WalkToTheVictoryPosition();
            StartCoroutine(levelManager.FinishRoom());
            
            enabled = false; // safety check to block multiple triggers.
        }
        else
        {
            StartCoroutine(levelManager.FinishRoom());
            AudioManager.singleton.Play("FinishRoom");
        }
    }
}
