using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private UIManager uIManager;
    [SerializeField]
    private SoundManager soundManager;
    [SerializeField]
    private GameObject player;
    private Transform spawnPoint;
    public CinemachineConfiner2D confiner2D;
    public List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        uIManager = FindObjectOfType<UIManager>();
        soundManager = FindObjectOfType<SoundManager>();
        player = GameObject.FindWithTag("Player");
        confiner2D = FindObjectOfType<CinemachineConfiner2D>();
    }
    /// <summary>
    /// OnSceneLoaded event
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        spawnPoint = GameObject.FindWithTag("Spawn").transform;
        player.transform.position = spawnPoint.position;
        confiner2D.m_BoundingShape2D = GameObject.FindWithTag("Confiner").GetComponent<Collider2D>();
        confiner2D.InvalidateCache();
        if(gameManager.gameState == GameManager.GameState.GamePlay)
        {
            soundManager.PlayMusic(1);
        }
        else if(gameManager.gameState == GameManager.GameState.MainMenu)
        {
            soundManager.PlayMusic(0);
            uIManager.startButton.Select();
        }
        else if(gameManager.gameState == GameManager.GameState.GameEnd)
        {
            soundManager.PlayMusic(2);
            uIManager.creditsBackButton.Select();
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Load target scene
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        switch(sceneName)
        {
            case "GamePlay":
                uIManager.UILoadingScreen(uIManager.HUD); 
                gameManager.ChangeGameState("GamePlay");
                break;
            case "MainMenu":
                uIManager.UILoadingScreen(uIManager.mainMenu); 
                gameManager.ChangeGameState("MainMenu");
                break;
            case "GameEnd":
                uIManager.UILoadingScreen(uIManager.creditsMenu);
                gameManager.ChangeGameState("GameEnd");
                break;
            default:
                uIManager.UILoadingScreen(uIManager.mainMenu); 
                gameManager.ChangeGameState("MainMenu");
                break;
        }
        StartCoroutine(WaitForScreenLoad(sceneName));
    }

    /// <summary>
    /// Waits for screen to load before starting operation. 
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator WaitForScreenLoad(string sceneName)
    {
        yield return new WaitForSeconds(uIManager.fadeTime);
        //Debug.Log("Loading Scene Starting");

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.completed += OperationCompleted;
        scenesToLoad.Add(operation);
    }
    /// <summary>
    /// Gets average progress for Loading bar. 
    /// </summary>
    /// <returns></returns>
    public float GetLoadingProgress()
    {
        float totalprogress = 0;

        foreach (AsyncOperation operation in scenesToLoad)
        {
            totalprogress += operation.progress;
        }

        return totalprogress / scenesToLoad.Count;
    }
    /// <summary>
    /// Event for when load operation is finished. 
    /// </summary>
    /// <param name="operation"></param>
    private void OperationCompleted(AsyncOperation operation)
    {
        scenesToLoad.Remove(operation);
        operation.completed -= OperationCompleted;
    }

    /// <summary>
    /// Quits Entire Game. 
    /// </summary>
    public void QuitGame()
    {
        //Debug line to test quit function in editor
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
