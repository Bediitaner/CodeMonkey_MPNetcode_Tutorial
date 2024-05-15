using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameManager : NetworkBehaviour
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

    public event EventHandler OnStateChangedEvent;
    public event EventHandler OnGamePausedEvent;
    public event EventHandler OnGameUnpausedEvent;
    public event EventHandler OnLocalPlayerReadyChangedEvent;

    #endregion

    #region Fields

    private bool localPlayerReady;
    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private float gamePlayingTimerMax = 90f;
    private bool isGamePaused = false;
    private Dictionary<ulong, bool> playerReadyDictionary;

    #endregion

 
    #region Unity: Awake | Start | Update

    private void Awake()
    {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        AddEvents();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f)
                {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                }

                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f)
                {
                    state.Value = State.GameOver;
                }

                break;
            case State.GameOver:
                break;
        }
    }

    #endregion

    #region Netcode: OnNetworkSpawn

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += OnStateValueChanged;
    }

    #endregion


    #region Toggle: PauseGame

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0f;

            OnGamePausedEvent?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;

            OnGameUnpausedEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion


    #region Is: LocalPlayerReady

    public bool IsLocalPlayerReady()
    {
        return localPlayerReady;
    }

    #endregion

    #region Is: GamePlaying

    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    #endregion

    #region Is: CountdownToStartActive

    public bool IsCountdownToStartActive()
    {
        return state.Value == State.CountdownToStart;
    }

    #endregion

    #region Is: GameOver

    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }

    #endregion


    #region Get: CountdownToStartTimer

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    #endregion

    #region Get: GamePlayingTimerNormalized

    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }

    #endregion


    #region ServerRpc: Set: LocalPlayerReady

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                // this player is not ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            state.Value = State.CountdownToStart;
        }

        Debug.Log("allClientsReady: " + allClientsReady);
    }

    #endregion

    
    #region Event: OnStateValueChanged

    private void OnStateValueChanged(State previousvalue, State newvalue)
    {
        OnStateChangedEvent?.Invoke(this, EventArgs.Empty);
    }

    #endregion
    
    #region Event: OnInteractAction

    private void InteractActionEvent(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
            localPlayerReady = true;

            SetPlayerReadyServerRpc();

            OnLocalPlayerReadyChangedEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion

    #region Event: OnPauseAction

    private void PauseActionEvent(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    #endregion

    #region Events: Add | Remove

    private void AddEvents()
    {
        GameInput.Instance.OnPauseActionEvent += PauseActionEvent;
        GameInput.Instance.OnInteractActionEvent += InteractActionEvent;
    }

    private void RemoveEvents()
    {
        GameInput.Instance.OnPauseActionEvent -= PauseActionEvent;
        GameInput.Instance.OnInteractActionEvent -= InteractActionEvent;
    }

    #endregion
}