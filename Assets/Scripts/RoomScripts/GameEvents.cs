using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    void Awake()
    {
        current = this;
    }

    public event Action<int> onDoorwayTriggerEnter;
    public void DoorwayTriggerEnter(int id)
    {
        if(onDoorwayTriggerEnter != null)
        {
            onDoorwayTriggerEnter(id);
        }
    }

    public event Action<int> onDoorwayTriggerExit;
    public void DoorwayTriggerExit(int id)
    {
        if (onDoorwayTriggerExit != null)
        {
            onDoorwayTriggerExit(id);
        }
    }

    public event Action<int> onTrapTriggerEnter;
    public void TrapTriggerEnter(int id)
    {
        if (onTrapTriggerEnter != null)
        {
            onTrapTriggerEnter(id);
        }
    }

    public event Action<int> onTrapTriggerExit;
    public void TrapTriggerExit(int id)
    {
        if (onTrapTriggerExit != null)
        {
            onTrapTriggerExit(id);
        }
    }
}
