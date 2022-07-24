using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour 
{
    public float rotateSpeed = 2f;
    public int objectId;
    public bool enabledAtStart = false;
    public bool backAndForthMovement = false;

    //Private:
    public Vector3 objectClosedPosition;
    public Vector3 objectOpenPosition;
    private bool objectIsOpening = false;
    private bool objectIsClosing = false;

    void Start()
    {        
        if (enabledAtStart)
        {
            transform.localPosition = objectClosedPosition;
        }
        else
        {
            transform.localPosition = objectClosedPosition;
        }
        GameEvents.current.onDoorwayTriggerEnter += OnObjectWayOpen;
        GameEvents.current.onDoorwayTriggerExit += OnObjectWayClose;
    }

    private void OnObjectWayOpen(int id)
    {
        Debug.Log("OnObjectWayOpen");
        if (id == objectId && !objectIsOpening && transform.localPosition != objectOpenPosition)
        {
            StartCoroutine(OpenObject());
        }       
    }
    private void OnObjectWayClose(int id)
    {
        Debug.Log("OnObjectWayClose");

        if (id == objectId && !objectIsClosing && transform.localPosition != objectClosedPosition)
        {
            StartCoroutine(CloseObject());
        }    
    }

    public IEnumerator OpenObject()
    {
        objectIsOpening = true;
        objectIsClosing = false;

        while (transform.localPosition != objectOpenPosition && !objectIsClosing)
        {
            Debug.Log("opening");
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, objectOpenPosition, rotateSpeed * Time.deltaTime);
            yield return null;
        }
        objectIsOpening = false;
        yield return null;
    }

    public IEnumerator CloseObject()
    {
        objectIsOpening = false;
        objectIsClosing = true;

        while (transform.localPosition != objectClosedPosition && !objectIsOpening)
        {
            Debug.Log("closing");
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, objectClosedPosition, rotateSpeed * Time.deltaTime);
            yield return null;
        }
        objectIsClosing = false;
        yield return null;
    }

    private void OnDestroy()
    {
        GameEvents.current.onDoorwayTriggerEnter -= OnObjectWayOpen;
        GameEvents.current.onDoorwayTriggerExit -= OnObjectWayClose;
    }
}
