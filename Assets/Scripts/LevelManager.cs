using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager singleton;

    [HideInInspector]
    public int currentRoomIndex = 0;

    private GameObject pirate;
    private CharacterController pirateCharacterController;
    private PirateController pirateControllerScipt;
    public GameObject levelEnvironmentPrefab;
    public GameObject currentRoom;

    public List<string> roomScenes;
    public List<string> roomScenesPirate1;
    public List<string> roomScenesPirate2;

    //public List<GameObject> rooms;
    private NavMeshSurface[] navMeshSurfaces;
    public Vector3 pirateStartPosition;
    public Vector3 cameraStartPosition;
    public GameObject finalEnvironmentPrefab;

    //UI:
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI roomText;
    public TextMeshProUGUI livesText;
    public GameObject pauseButton;

    public GameObject finishedMenu;
    public GameObject labelNewBest;
    public TextMeshProUGUI textTotalTime;

    public GameObject revivePanel;
    public TextMeshProUGUI labelGameOver;
    public GameObject buttonGameOverContinue;
    public GameObject buttonReviveContinue;

    public GameObject continueButton;
    public TextMeshProUGUI hintText;
    public CanvasGroup faderCanvasGroup;

    //Stats:
    private float currentRoomTime = 0f;
    private float totalTime = 0f;
    private bool stopTimer = false;

    private void Awake()
    {
        singleton = this;
        AudioManager.singleton.Play("Gameplay");
        pirate = GameObject.FindGameObjectWithTag("Player");
        pirateCharacterController = pirate.GetComponent<CharacterController>();
        pirateControllerScipt = pirate.GetComponent<PirateController>();

        if (GameData.singleton != null)
        {
            switch (GameData.singleton.selectedPirateIndex)
            {
                case 0: roomScenes = roomScenesPirate1; break;
                case 1: roomScenes = roomScenesPirate2; break;
                default: break;
            }
        }
        else
        {
            roomScenes = roomScenesPirate1;
        }

        StartCoroutine(LoadRoomScene(false));
        continueButton.SetActive(false);
    }

    private void Start()
    {
        roomText.text = "Room: 1/" + roomScenes.Count;
        livesText.text = "Lives: " + pirateControllerScipt.pirateLives;

        if (roomScenes[0].Equals("Room0"))
        {
            hintText.text = "Run using WASD / arrow keys.";
            hintText.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (!stopTimer)
        {
            currentRoomTime += Time.deltaTime;
            totalTime += Time.deltaTime;
            DisplayTime(totalTime);
        }
    }

    public IEnumerator FinishRoom()
    {
        stopTimer = true;
        hintText.text = "";
        Debug.Log("Room " + (currentRoomIndex + 1) + " cleared in " + currentRoomTime.ToString() + "!");
        currentRoomTime = 0f;

        if(currentRoomIndex < (roomScenes.Count - 1))
        {
            //// Spawn new room: ////
            currentRoomIndex++;
            yield return StartCoroutine(LoadRoomScene(false));
        }
        else
        {
            //// Victory: ////
            SaveData();          
        }

        
        yield return null;
    }

    private void SaveData()
    {
        textTotalTime.text = timerText.text;
        labelNewBest.SetActive(false);
        timerText.transform.parent.gameObject.SetActive(false);
        pauseButton.SetActive(false);

        Debug.Log("SaveData");
        switch (GameData.singleton.selectedPirateIndex)
        {
            case 0: GameData.singleton.pirate1NumberOfEscapes++; 
                if(GameData.singleton.pirate1BestTime > totalTime || GameData.singleton.pirate1BestTime == 0)
                {
                    GameData.singleton.pirate1BestTime = totalTime;
                    labelNewBest.SetActive(true);
                    Debug.Log("new high score!");
                }
                    break;
            case 1: GameData.singleton.pirate2NumberOfEscapes++;
                if (GameData.singleton.pirate2BestTime > totalTime || GameData.singleton.pirate2BestTime == 0)
                {
                    GameData.singleton.pirate2BestTime = totalTime;
                    labelNewBest.SetActive(true);
                    Debug.Log("new high score!");
                }
                break;
            case 2: GameData.singleton.pirate3NumberOfEscapes++;
                if (GameData.singleton.pirate3BestTime > totalTime || GameData.singleton.pirate3BestTime == 0)
                {
                    GameData.singleton.pirate3BestTime = totalTime;
                    labelNewBest.SetActive(true);
                    Debug.Log("new high score!");
                }
                break;
            case 3: GameData.singleton.pirate4NumberOfEscapes++;
                if (GameData.singleton.pirate4BestTime > totalTime || GameData.singleton.pirate4BestTime == 0)
                {
                    GameData.singleton.pirate4BestTime = totalTime;
                    labelNewBest.SetActive(true);
                    Debug.Log("new high score!");
                }
                break;

            default: break;
        }

        SaveSystem.SaveGame();
    }

    private IEnumerator LoadRoomScene(bool reloadCurrentScene)
    {
        yield return StartCoroutine(Fade(1));

        pirateCharacterController.enabled = false;

        roomText.text = "Room: " + (currentRoomIndex + 1) + "/" + roomScenes.Count;

        Scene currentRoomScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        AsyncOperation operationUnloadPreviousRoom = null;
        AsyncOperation operationLoadFinalRoom = null;
        AsyncOperation operationLoadNewRoom = null;

        //// Unload old scene ////
        if (currentRoomIndex > 0 || reloadCurrentScene)
        {
            Debug.Log("unload scene");

            /*GameObject[] skellies = GameObject.FindGameObjectsWithTag("Skelly");
            foreach(GameObject skelly in skellies)
            {
                skelly.GetComponent<Skelly>().enabled = false;
                skelly.GetComponent<NavMeshAgent>().enabled = false;
            }*/
            operationUnloadPreviousRoom = SceneManager.UnloadSceneAsync(currentRoomScene);
        }

        //// FinalRoom /////
        if (currentRoomIndex == (roomScenes.Count - 1))
        {
            //Instantiate(finalEnvironmentPrefab);
            Camera.main.GetComponent<CameraFollow>().clampForward = 10;

            operationLoadFinalRoom = SceneManager.LoadSceneAsync("RoomFinal", LoadSceneMode.Additive);
        }
        else
        {
            Camera.main.GetComponent<CameraFollow>().clampForward = -0.5f;
        }

        //// Load new scene ////
        operationLoadNewRoom = SceneManager.LoadSceneAsync(roomScenes[currentRoomIndex], LoadSceneMode.Additive);

        if(operationUnloadPreviousRoom != null)
        {
            while (!operationUnloadPreviousRoom.isDone)
            {
                yield return null;
            }
        }
        if (operationLoadFinalRoom != null)
        {
            while (!operationLoadFinalRoom.isDone)
            {
                yield return null;
            }
        }
        if (operationLoadNewRoom != null)
        {
            while (!operationLoadNewRoom.isDone)
            {
                //Debug.Log(operation1.progress);

                // float progress = Mathf.Clamp01(operation.progress / .9f);
                //loadingBar.value = progress;
                //progressText.text = progress * 100f + "%";
                yield return null;
            }
        }

        StartCoroutine(Fade(0));

        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newlyLoadedScene);

        //Build NavMesh again:
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        navMeshSurfaces = new NavMeshSurface[floors.Length];

        for (int i = 0; i < floors.Length; i++)
        {
            navMeshSurfaces[i] = floors[i].GetComponent<NavMeshSurface>();
        }
        Debug.Log("building navmesh");
        navMeshSurfaces[0].BuildNavMesh();

        Camera.main.transform.position = cameraStartPosition;
        pirate.transform.position = pirateStartPosition;
        pirateCharacterController.enabled = true;
        if (reloadCurrentScene)
        {
            pirateControllerScipt.Revive();
        }


        stopTimer = false;

        yield return null;
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliSeconds = (timeToDisplay % 1) * 100;
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliSeconds);
    }

    public void LoseOneLife()
    {
        livesText.text = "Lives: " + pirateControllerScipt.pirateLives;

        revivePanel.SetActive(true);
        labelGameOver.text = "YOU DIED";
        buttonReviveContinue.SetActive(true);
        buttonReviveContinue.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Revive (" + pirateControllerScipt.pirateLives + ")";
        buttonGameOverContinue.SetActive(false);
        stopTimer = true;
    }
    public void LoseGame()
    {
        livesText.text = "Lives: " + pirateControllerScipt.pirateLives;

        revivePanel.SetActive(true);
        labelGameOver.text = "GAME OVER";
        buttonReviveContinue.SetActive(false);
        buttonGameOverContinue.SetActive(true);
        stopTimer = true;
    }
    public void RevivePirate()
    {
        revivePanel.SetActive(false);
        pirateCharacterController.enabled = false;

        StartCoroutine(LoadRoomScene(true));

        stopTimer = false;
        Time.timeScale = 1;
    }

    public void ChangeSceneToMenu()
    {
        SceneManager.LoadScene("Menu");
        AudioManager.singleton.Stop("Gameplay");
    }

    public void PauseGame()
    {
        stopTimer = true;
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        stopTimer = false;
        Time.timeScale = 1;
    }
    private IEnumerator Fade(float finalAlpha)
    {
        faderCanvasGroup.blocksRaycasts = true;

        if (finalAlpha == 1)
        {
            while (faderCanvasGroup.alpha < finalAlpha)
            {
                faderCanvasGroup.alpha += Time.deltaTime * 2;
                yield return null;
            }
        }
        else
        {
            while (faderCanvasGroup.alpha > finalAlpha)
            {
                faderCanvasGroup.alpha -=  Time.deltaTime * 2;
                yield return null;
            }
        }

        faderCanvasGroup.blocksRaycasts = false;
    }
}
