using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour 
{
    public float objectSpeed = 2f;
    public int objectId;
    public bool startingOpen = false;

    public bool backAndForthMovement = false;
    public bool movementEnabledOnTheStart = false;

    public Vector3 objectClosedPosition;
    public Vector3 objectOpenPosition;

    public bool isDoor = false;
    //Rotate:
    public float rotateTime;
    public bool rotateInstedOfMoving = false;
    public float objectClosedYRotation;
    public float objectOpenYRotation;

    public bool testOpen = false;
    public bool testClose = false;

    public AudioClip openSound;
    public AudioClip closeSound;

    //Private:
    private bool objectIsOpening = false;
    private bool objectIsClosing = false;
    private AudioSource omAudioSource;
    private bool objectState;

    void Start()
    {
        omAudioSource = GetComponent<AudioSource>();
        if (startingOpen)
        {
            objectState = true;
            if (rotateInstedOfMoving)
            {
                transform.localRotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, objectOpenYRotation, transform.eulerAngles.z));
            }
            else
            {
                transform.localPosition = objectOpenPosition;
            }
            if (movementEnabledOnTheStart)
            {
                StartCoroutine(CloseObject());
            }
        }
        else
        {
            objectState = false;

            if (rotateInstedOfMoving)
            {
                transform.localRotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, objectClosedYRotation, transform.eulerAngles.z));
            }
            else
            {
                transform.localPosition = objectClosedPosition;
            }
            if (movementEnabledOnTheStart)
            {
                StartCoroutine(OpenObject());
            }
        }
        GameEvents.current.onDoorwayTriggerEnter += OnObjectWayOpen;
        GameEvents.current.onDoorwayTriggerExit += OnObjectWayClose;
    }

    private void Update()
    {
        if (testClose)
        {
            StartCoroutine(CloseObject());
            testClose = false;
        }
        else if (testOpen)
        {
            StartCoroutine(OpenObject());
            testOpen = false;
        }
    }

    private void OnObjectWayOpen(int id)
    {
        Debug.Log("OnObjectWayOpen");
        if (id == objectId && !objectIsOpening)
        {
            if (rotateInstedOfMoving && (transform.eulerAngles.y != objectOpenYRotation))
            {
                StartCoroutine(OpenObject());
            }
            if(!rotateInstedOfMoving && (transform.localPosition != objectOpenPosition))
            {
                StartCoroutine(OpenObject());
            }
        }       
    }
    private void OnObjectWayClose(int id)
    {
        Debug.Log("OnObjectWayClose");

        if (id == objectId && !objectIsClosing)
        {
            if (rotateInstedOfMoving && (transform.eulerAngles.y != objectClosedYRotation))
            {
                StartCoroutine(CloseObject());
            }
            if(!rotateInstedOfMoving && (transform.localPosition != objectClosedPosition))
            {
                StartCoroutine(CloseObject());
            }
        }    
    }

    public IEnumerator BackAndForth(bool startWithOpen)
    {
        if (startWithOpen)
        {
            while (backAndForthMovement)
            {
                yield return StartCoroutine(OpenObject());
                yield return StartCoroutine(CloseObject());
            }
        }
        else
        {
            while (backAndForthMovement)
            {
                yield return StartCoroutine(CloseObject());
                yield return StartCoroutine(OpenObject());
            }
        }
    }

    public IEnumerator OpenObject()
    {
        objectIsOpening = true;
        objectIsClosing = false;

        if (rotateInstedOfMoving)
        {
            float startRotation = transform.eulerAngles.y;
            float endRotation = objectOpenYRotation;
            float t = 0.0f;
            while (t < rotateTime && !objectIsClosing)
            {
                t += Time.deltaTime;
                float yRotation = Mathf.Lerp(startRotation, endRotation, t / rotateTime) % 360.0f;
                transform.localRotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z));
                yield return null;
            }
        }
        else
        {
            if (isDoor)
            {
                omAudioSource.clip = openSound;
                omAudioSource.Play();
            }
            while (transform.localPosition != objectOpenPosition && !objectIsClosing)
            {
                Debug.Log("opening");
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, objectOpenPosition, objectSpeed * Time.deltaTime);
                yield return null;
            }
        }  

        if (backAndForthMovement)
        {
            StartCoroutine(CloseObject());
        }

        objectIsOpening = false;
        objectState = true;
        yield return null;
    }

    public IEnumerator CloseObject()
    {
        objectIsOpening = false;
        objectIsClosing = true;

        if (rotateInstedOfMoving)
        {
            float startRotation = transform.eulerAngles.y;
            float endRotation = objectClosedYRotation;
            float t = 0.0f;
            while (t < rotateTime && !objectIsOpening)
            {
                t += Time.deltaTime;
                float yRotation = Mathf.Lerp(startRotation, endRotation, t / rotateTime) % 360.0f;
                transform.localRotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z));
                yield return null;
            }
        }
        else
        {
            if (isDoor)
            {
                omAudioSource.clip = closeSound;
                omAudioSource.Play();
            }
            while (transform.localPosition != objectClosedPosition && !objectIsOpening)
            {
                Debug.Log("closing");
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, objectClosedPosition, objectSpeed * Time.deltaTime);
                yield return null;
            }
        }

        if (backAndForthMovement)
        {
            StartCoroutine(OpenObject());
        }

        objectIsClosing = false;
        objectState = false;
        yield return null;
    }

    private void OnDestroy()
    {
        GameEvents.current.onDoorwayTriggerEnter -= OnObjectWayOpen;
        GameEvents.current.onDoorwayTriggerExit -= OnObjectWayClose;
    }
}
