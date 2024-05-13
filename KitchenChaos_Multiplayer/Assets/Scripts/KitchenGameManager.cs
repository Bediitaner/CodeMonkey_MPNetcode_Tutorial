using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    #region Singleton

    public static KitchenGameManager Instance { get; private set; }

    #endregion

    #region Events

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    #endregion
    
    #region Fields

    private State state;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 300f;
    private bool isGamePaused = false;

    #endregion


    #region Unity: Awake | Start | Update

    private void Awake()
    {
        Instance = this;

        state = State.WaitingToStart;
    }

    private void Start()
    {
        AddEvents();

        //DEBUG TRIGGER GAME START AUTOMATICALLY
        state = State.CountdownToStart;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
    
    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;
            case State.GameOver:
                break;
        }
    }

    #endregion


    #region Toggle: PauseGame

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0f;

            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;

            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion

    
    #region Is: GamePlaying

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }
    

    #endregion

    #region Is: CountdownToStartActive

    public bool IsCountdownToStartActive()
    {
        return state == State.CountdownToStart;
    }

    #endregion
    
    #region Is: GameOver

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    #endregion
    
    #region Get: CountdownToStartTimer

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }

    #endregion

    #region Get: GamePlayingTimerNormalized

    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
    }

    #endregion


    
    #region Event: OnInteractAction

    private void OnInteractAction(object sender, EventArgs e)
    {
        if (state == State.WaitingToStart)
        {
            state = State.CountdownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion

    #region Event: OnPauseAction

    private void OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    #endregion
    
    #region Events: Add | Remove

    private void AddEvents()
    {
        GameInput.Instance.OnPauseAction += OnPauseAction;
        GameInput.Instance.OnInteractAction += OnInteractAction;
    }

    private void RemoveEvents()
    {
        GameInput.Instance.OnPauseAction -= OnPauseAction;
        GameInput.Instance.OnInteractAction -= OnInteractAction;
    }

    #endregion
}