using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public enum GameState{MainMenu, GamePlay, Paused, Options, GameEnd}
    public GameState gameState;
    private GameState previousState;
    [Header("Managers")]
    public UIManager uIManager;
    public SoundManager soundManager;
    public LevelManager levelManager;
    [Header("Player")]
    public PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        if(gameManager != null)
        {
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
            gameManager = this;
        }
        uIManager = FindObjectOfType<UIManager>();
        levelManager = FindObjectOfType<LevelManager>();
        soundManager = FindObjectOfType<SoundManager>();
        player = FindObjectOfType<PlayerController>();
        ChangeGameState("MainMenu");
    }

    /// <summary>
    /// Changes GameState to desired State, runs state initiation function.
    /// </summary>
    /// <param name="state"></param>
    public void ChangeGameState(string state)
    {
        previousState = gameState;
        switch(state)
        {
            case "MainMenu":
                gameState = GameState.MainMenu;
                MainMenu();
                break;
            case "GamePlay":
                gameState = GameState.GamePlay;
                GamePlay();
                break;
            case "Pause":
                gameState = GameState.Paused;
                Pause();
                break;
            case "Options":
                gameState = GameState.Options;
                Options();
                break;
            case "GameEnd":
                gameState = GameState.GameEnd;
                GameEnd();
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    void MainMenu()
    {
        player.RB2D.simulated = false;
    }

    void GamePlay()
    {
        player.RB2D.simulated = true;
    }

    void Options()
    {

    }

    void Pause()
    {

    }

    void GameEnd()
    {

    }
}
