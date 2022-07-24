using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneController : MonoBehaviour
{
    public static SceneController singleton;

    public event Action BeforeSceneUnload;
    public event Action AfterSceneLoad;

    public CanvasGroup faderCanvasGroup;
    public float fadeDuration = 1f;
    public string startingSceneName;
    private bool isFading;

    public GameObject loadingScreen;
    public Slider loadingBar;
    public TextMeshProUGUI progressText;

    public void ChangeToDifferentScene(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    void Awake()
    {
        singleton = this;
    }

    private IEnumerator Start()
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Persistent"))
        {
            faderCanvasGroup.alpha = 1f;
            yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName));
            StartCoroutine(Fade(0f));
        }

        yield return null;
    }

    public void FadeAndLoadScene(string sceneName)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName));
        }
    }

    private IEnumerator FadeAndSwitchScenes(string sceneName)
    {
        yield return StartCoroutine(Fade(1f));
        if (BeforeSceneUnload != null)
        {
            BeforeSceneUnload();
        }
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));
        if (AfterSceneLoad != null)
        {
            AfterSceneLoad();
        }
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            Debug.Log(operation.progress);

            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingBar.value = progress;
            progressText.text = progress * 100f + "%";
            yield return null;      
        }

        loadingScreen.SetActive(false);

        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    private IEnumerator Fade(float finalAlpha)
    {
        isFading = true;
        faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = faderCanvasGroup.alpha - finalAlpha / fadeDuration;
        if(finalAlpha == 0)
        {
            while (faderCanvasGroup.alpha > finalAlpha)
            {
                faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            while (faderCanvasGroup.alpha < finalAlpha)
            {
                faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
                yield return null;
            }
        }
        

        isFading = false;
        faderCanvasGroup.blocksRaycasts = false;
    }
}
