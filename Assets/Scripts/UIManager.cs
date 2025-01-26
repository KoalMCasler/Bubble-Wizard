using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private LevelManager levelManager;
    [SerializeField]
    private SoundManager soundManager;
    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;
    public GameObject HUD;
    [Header("Loading Screen UI Elements")]
    public GameObject loadingScreen;
    public CanvasGroup loadingScreenCanvasGroup;
    public Image loadingBar;
    public float fadeTime;
    [Header("Options Menu")]
    public Slider masterVolSlider;
    public Slider musicVolSlider;
    public Slider sFXVolSlider;
    [Header("Buttons")]
    public Button startButton;
    public Button backButton;
    public Button creditsBackButton;
    public Button resumeButton;
    [Header("HUD")]
    public Slider progressBar;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
        soundManager = FindObjectOfType<SoundManager>();
        SetUIMenu();
        GetStartingVolume();
    }

    void Update()
    {
        if(gameManager.gameState == GameManager.GameState.GamePlay)
        {
            progressBar.value = gameManager.player.transform.position.x;
        }
    }
    /// <summary>
    /// Clears all menus for activating the one you want.
    /// </summary>
    void ResetAllMenus()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        HUD.SetActive(false);
    }
    /// <summary>
    /// Sets UI to main Menu.
    /// </summary>
    public void SetUIMenu()
    {
        ResetAllMenus();
        startButton.Select();
        mainMenu.SetActive(true);
    }
    /// <summary>
    /// Set UI to pause menu. 
    /// </summary>
    public void SetUIPause()
    {
        ResetAllMenus();
        resumeButton.Select();
        pauseMenu.SetActive(true);
    }
    /// <summary>
    /// Sets UI to options. Changes state so it can retune to correct space.
    /// </summary>
    public void SetUIOptions()
    {
        ResetAllMenus();
        backButton.Select();
        gameManager.ChangeGameState("Options");
        optionsMenu.SetActive(true);
    }

    /// <summary>
    /// Sets HUD active for gameplay.
    /// </summary>
    public void SetUIHUD()
    {
        ResetAllMenus();
        HUD.SetActive(true);
    }

    /// <summary>
    /// Sets UI to credits.
    /// </summary>
    public void SetUICredits()
    {
        ResetAllMenus();
        creditsBackButton.Select();
        creditsMenu.SetActive(true);
    }

    /// <summary>
    /// Used to go back from options to ether main menu or pause menu.
    /// </summary>
    public void BackFromOptions()
    {
        gameManager.BackToPrevState();
        switch(gameManager.gameState)
        {
            case GameManager.GameState.MainMenu:
                SetUIMenu();
                break;
            case GameManager.GameState.Paused:
                SetUIPause();
                break;
        }
    }

    /// <summary>
    /// Starts UI loading screen process.
    /// </summary>
    /// <param name="targetPanel"></param>
    public void UILoadingScreen(GameObject targetPanel)
    {
        StartCoroutine(LoadingUIFadeIN());
        StartCoroutine(DelayedSwitchUIPanel(fadeTime, targetPanel));
    }

    /// <summary>
    /// Fades loading scnreen out.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadingUIFadeOut()
    {
        //Debug.Log("Starting Fadeout");

        float timer = 0;

        while (timer < fadeTime)
        {
            loadingScreenCanvasGroup.alpha = Mathf.Lerp(1, 0, timer/fadeTime);
            timer += Time.deltaTime;
            yield return null;
        }

        loadingScreenCanvasGroup.alpha = 0;
        loadingScreen.SetActive(false);
        loadingBar.fillAmount = 0;
        //Debug.Log("Ending Fadeout");
    }
    /// <summary>
    /// Fades Loading screen in.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadingUIFadeIN()
    {
        //Debug.Log("Starting Fadein");
        float timer = 0;
        loadingScreen.SetActive(true);

        while (timer < fadeTime)
        {
            loadingScreenCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeTime);
            timer += Time.deltaTime;
            yield return null;
        }

        loadingScreenCanvasGroup.alpha = 1;

        //Debug.Log("Ending Fadein");
        StartCoroutine(LoadingBarProgress());
    }
    /// <summary>
    /// Sets the loading bar progress to average progress of all loading. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadingBarProgress()
    {
        //Debug.Log("Starting Progress Bar");
        while (levelManager.scenesToLoad.Count <= 0)
        {
            //waiting for loading to begin
            yield return null;
        }
        while (levelManager.scenesToLoad.Count > 0)
        {
            loadingBar.fillAmount = levelManager.GetLoadingProgress();
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        //Debug.Log("Ending Progress Bar");
        StartCoroutine(LoadingUIFadeOut());
    }
    /// <summary>
    /// used for fade in fade out for loading screen UI. 
    /// </summary>
    /// <param name="time"></param>
    /// <param name="uiPanel"></param>
    /// <returns></returns>
    private IEnumerator DelayedSwitchUIPanel(float time, GameObject uiPanel)
    {
        yield return new WaitForSeconds(time);
        ResetAllMenus();
        uiPanel.SetActive(true);
    }
    /// <summary>
    /// Gets starting volume from mixer and applies them to sliders. 
    /// </summary>
    public void GetStartingVolume()
    {
        if(soundManager.mixer.GetFloat("MasterVol",out float masterValue))
        {
            masterVolSlider.value = masterValue;
        }
        if(soundManager.mixer.GetFloat("MusicVol",out float musicValue))
        {
            musicVolSlider.value = musicValue;
        }
        if(soundManager.mixer.GetFloat("SFXVol", out float sfxValue))
        {
            sFXVolSlider.value = sfxValue;
        }
    }
    /// <summary>
    /// Used by slider to pass value to sound manager
    /// </summary>
    /// <param name="group"></param>
    public void SliderVolume(string group)
    {
        switch(group)
        {
            case "MasterVol":
                soundManager.ChangeVolume(group,masterVolSlider.value);
                break;
            case "MusicVol":
                soundManager.ChangeVolume(group,musicVolSlider.value);
                break;
            case "SFXVol":
                soundManager.ChangeVolume(group,sFXVolSlider.value);
                break;
        }
    }

    public void BackFromCredits()
    {
        if(gameManager.gameState == GameManager.GameState.MainMenu)
        {
            SetUIMenu();
        }
        else if(gameManager.gameState == GameManager.GameState.GameEnd)
        {
            levelManager.LoadScene("MainMenu");
        }
    }
}
