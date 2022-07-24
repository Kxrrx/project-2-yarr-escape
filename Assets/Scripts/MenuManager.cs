using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System;

public class MenuManager : MonoBehaviour
{
    private SceneController sceneController;
    public PlayableDirector pirate1Director;
    public PlayableDirector pirate2Director;

    public Transform[] cameraPositions;

    public Transform[] prisonCellCameraPositions;
    public GameObject browseRightButton;
    public GameObject browseLeftButton;
    public GameObject startButton;
    public TextMeshProUGUI labelPirateName;
    public TextMeshProUGUI labelNumberOfEscapes;
    public TextMeshProUGUI labelBestTime;

    private bool showControlsText = true;
    public Button showHideControlsButton;
    public TextMeshProUGUI showHideControlsButtonText;
    public GameObject labelControls;

    public int currentPrisonCell = 0;

    private void Start()
    {
        SaveSystem.LoadGame();
        if(GameData.singleton.pirate1NumberOfEscapes < 1)
        {
            showControlsText = true;
            showHideControlsButtonText.text = "Hide controls";
            labelControls.SetActive(true);
        }
        else
        {
            showControlsText = false;
            showHideControlsButtonText.text = "Show controls";
            labelControls.SetActive(false);
        }

        AudioManager.singleton.Stop("Gameplay");
        AudioManager.singleton.Play("Menu");

        browseLeftButton.gameObject.SetActive(false);
        ShowPirateInfo();

    }

    public void ShowHideControls()
    {
        if (showControlsText)
        {
            showControlsText = false;
            showHideControlsButtonText.text = "Show controls";
        }
        else
        {
            showControlsText = true;
            showHideControlsButtonText.text = "Hide controls";
        }
        labelControls.SetActive(showControlsText);

    }

    public void ShowNextPrisonCell()
    {
        currentPrisonCell++;
        StopAllCoroutines();
        StartCoroutine(MoveCameraToNextPrisonCell(currentPrisonCell));
    }
    public void ShowPreviousPrisonCell()
    {
        currentPrisonCell--;
        StopAllCoroutines();
        StartCoroutine(MoveCameraToPreviousPrisonCell(currentPrisonCell));
    }

    private IEnumerator MoveCameraToNextPrisonCell(int prisonCellIndex)
    {
        ShowPirateInfo();

        browseLeftButton.gameObject.SetActive(true);

        if (currentPrisonCell == prisonCellCameraPositions.Length - 1)
        {
            browseRightButton.gameObject.SetActive(false);
        }

        float elapsedTime = 0;
        float lerpTime = 1f;

        while (elapsedTime < lerpTime)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraPositions[prisonCellIndex].position, (elapsedTime / lerpTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    private IEnumerator MoveCameraToPreviousPrisonCell(int prisonCellIndex)
    {
        ShowPirateInfo();

        browseRightButton.gameObject.SetActive(true);

        if (currentPrisonCell == 0)
        {
            browseLeftButton.gameObject.SetActive(false);
        }

        float elapsedTime = 0;
        float lerpTime = 1f;

        while (elapsedTime < lerpTime)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraPositions[prisonCellIndex].position, elapsedTime / lerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    private void ShowPirateInfo()
    {
        Button startBtn = startButton.GetComponent<Button>();

        switch (currentPrisonCell)
        {
            case 0: labelPirateName.text = GameData.singleton.pirate1Name;
                    ShowNumberOfEscapes(GameData.singleton.pirate1NumberOfEscapes);
                    ShowBestTime(GameData.singleton.pirate1BestTime);
                    startButton.SetActive(true);
                    startBtn.interactable = true;
                break;
            case 1: labelPirateName.text = GameData.singleton.pirate2Name;
                    ShowNumberOfEscapes(GameData.singleton.pirate2NumberOfEscapes);
                    startButton.SetActive(true);

                    if (GameData.singleton.pirate1NumberOfEscapes > 0)
                    {
                        startBtn.interactable = true;
                        ShowBestTime(GameData.singleton.pirate2BestTime);

                    }
                    else
                    {
                        labelNumberOfEscapes.text = "You must first escape with Robert the Robber.";
                        startBtn.interactable = false;
                    }
                     break;
            case 2: labelPirateName.text = GameData.singleton.pirate3Name;
                    ShowNumberOfEscapes(GameData.singleton.pirate3NumberOfEscapes);
                    ShowBestTime(GameData.singleton.pirate3BestTime);
                    labelNumberOfEscapes.text = "Coming soon...";
                    if (GameData.singleton.pirate2NumberOfEscapes > 0)
                    {
                        //startButton.SetActive(true);
                        startButton.SetActive(false);
                    }
                    else
                    {
                        startButton.SetActive(false);
                    }
                    break;
            case 3: labelPirateName.text = GameData.singleton.pirate4Name;
                    ShowNumberOfEscapes(GameData.singleton.pirate4NumberOfEscapes);
                    ShowBestTime(GameData.singleton.pirate4BestTime);
                    labelNumberOfEscapes.text = "Coming soon...";

                    if (GameData.singleton.pirate2NumberOfEscapes > 0)
                    {
                        //startButton.SetActive(true);
                        startButton.SetActive(false);
                    }
                    else
                    {
                        startButton.SetActive(false);
                    }
                        break;
            default: break;
        }
    }

    public void ShowNumberOfEscapes(int numberOfEscapes)
    {
        if(numberOfEscapes > 0)
        {
            labelNumberOfEscapes.text = "Escape count: " + numberOfEscapes;
        }
        else
        {
            labelNumberOfEscapes.text = "Has never escaped.";
        }
    }
    public void ShowBestTime(float bestTime)
    {
        Debug.Log(bestTime);
        if(bestTime > 0)
        {
            labelBestTime.text = "Best time: " + ConvertTimeToString(bestTime);
        }
        else
        {
            labelBestTime.text = "";
        }
    }

    public void ChangeToDifferentScene(string levelName)
    {
        sceneController = FindObjectOfType<SceneController>();
        sceneController.FadeAndLoadScene(levelName);
    }

    public void RestartThisScene()
    {
        sceneController = FindObjectOfType<SceneController>();
        sceneController.FadeAndLoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string levelName)
    {
        StartCoroutine(PlayEscapeCutscene(levelName));
    }

    public IEnumerator PlayEscapeCutscene(string levelName)
    {
        GameData.singleton.selectedPirateIndex = currentPrisonCell;
        SaveSystem.SaveGame();

        if(currentPrisonCell == 0)
        {
            pirate1Director.Play();
        }
        else if(currentPrisonCell == 1)
        {
            pirate2Director.Play();
        }
        else
        {
            throw new NotImplementedException();
        }

        yield return new WaitForSeconds((float)pirate1Director.duration);
        SceneManager.LoadScene(levelName);
        AudioManager.singleton.Stop("Menu");
        yield return null;
    }
    private string ConvertTimeToString(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        float milliSeconds = (time % 1) * 100;
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliSeconds);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
