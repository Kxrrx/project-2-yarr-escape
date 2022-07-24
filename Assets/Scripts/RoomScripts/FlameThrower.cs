using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : MonoBehaviour
{
    public int objectId;
    public bool enableTrapOnStart = false;
    private bool trapState = false;
    private ParticleSystem part;
    private PirateController pirateControllerScript;
    private AudioSource ftAudioSource;
    void Start()
    {
        pirateControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PirateController>();
        part = transform.GetComponent<ParticleSystem>();
        ftAudioSource = GetComponent<AudioSource>();
        if (enableTrapOnStart)
        {
            StartCoroutine(StartTrap());
        }
        else
        {
            StartCoroutine(StopTrap());
        }
        GameEvents.current.onTrapTriggerEnter += OnStartTrap;
        GameEvents.current.onTrapTriggerExit += OnStopTrap;
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Player")
        {
            pirateControllerScript.isDead = true;
        }
    }

    private void OnStartTrap(int id)
    {
        if (id == objectId && !trapState)
        {
            StartCoroutine(StartTrap());
        }
    }

    private IEnumerator StartTrap()
    {
        trapState = true;
        part.enableEmission = true;
        ftAudioSource.Play();
        yield return null;
    }

    private void OnStopTrap(int id)
    {
        if (id == objectId && trapState)
        {
            StartCoroutine(StopTrap());
        }
    }
    private IEnumerator StopTrap()
    {
        trapState = false;
        part.enableEmission = false;
        ftAudioSource.Stop();

        yield return null;
    }
    private void OnDestroy()
    {
        GameEvents.current.onTrapTriggerEnter -= OnStartTrap;
        GameEvents.current.onTrapTriggerExit -= OnStopTrap;
    }
}
