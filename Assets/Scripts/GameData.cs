using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData : MonoBehaviour
{
    public static GameData singleton;

    public int selectedPirateIndex; //0-3 for 4 pirates.

    public string pirate1Name = "Robert the Robber";
    public string pirate2Name = "Peteplank";
    public string pirate3Name = "Captain Crabby";
    public string pirate4Name = "Randell Rummy";

    public int pirate1NumberOfEscapes;
    public int pirate2NumberOfEscapes;
    public int pirate3NumberOfEscapes;
    public int pirate4NumberOfEscapes;

    public float pirate1BestTime;
    public float pirate2BestTime;
    public float pirate3BestTime;
    public float pirate4BestTime;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        singleton = this;
    }
}