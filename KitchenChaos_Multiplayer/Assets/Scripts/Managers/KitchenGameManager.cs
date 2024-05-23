using System;
using System.Collections.Generic;
using KitchenChaos_Multiplayer.Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KitchenChaos_Multiplayer.Managers
{
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
        public event EventHandler OnLocalGamePausedEvent;
        public event EventHandler OnLocalGameUnpausedEvent;
        public event EventHandler OnLocalPlayerReadyChangedEvent;

        public event EventHandler OnMultiplayerGamePausedEvent;
        public event EventHandler OnMultiplayerGameUnpausedEvent;

        #endregion

        #region Content

        [SerializeField] private Transform playerPrefab;

        #endregion
    
        #region Fields

        private float gamePlayingTimerMax = 90f;
        private bool localPlayerReady;
        private bool isLocalGamePaused = false;
        private bool autoTestGamePausedState;

        private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
        private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
        private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
        private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);

        private Dictionary<ulong, bool> playerReadyDictionary;
        private Dictionary<ulong, bool> playerPausedDictionary;

        #endregion


        #region Unity: Awake | Start | Update | LateUpdate

        private void Awake()
        {
            Instance = this;

            playerReadyDictionary = new Dictionary<ulong, bool>();
            playerPausedDictionary = new Dictionary<ulong, bool>();
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

        private void LateUpdate()
        {
            if (autoTestGamePausedState)
            {
                autoTestGamePausedState = false;
                TestGamePausedState();
            }
        }

        #endregion

        #region Netcode: OnNetworkSpawn

        public override void OnNetworkSpawn()
        {
            state.OnValueChanged += OnStateValueChanged;
            isGamePaused.OnValueChanged += OnPauseValueChanged;

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoadComplete;
            }
        }

        private void OnSceneLoadComplete(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
        {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var playerTransform = Instantiate(playerPrefab);
                playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            }
        }

        #endregion


        #region Toggle: PauseGame

        public void TogglePauseGame()
        {
            isLocalGamePaused = !isLocalGamePaused;
            if (isLocalGamePaused)
            {
                PauseGameServerRpc();

                OnLocalGamePausedEvent?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                UnPauseGameServerRpc();

                OnLocalGameUnpausedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region ServerRpc: Pause: Game

        [ServerRpc(RequireOwnership = false)]
        private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
        {
            playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
            TestGamePausedState();
        }

        #endregion

        #region ServerRpc: UnPause: Game

        [ServerRpc(RequireOwnership = false)]
        private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default)
        {
            playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
            TestGamePausedState();
        }

        #endregion

        #region Test: GamePausedState

        private void TestGamePausedState()
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
                {
                    isGamePaused.Value = true;
                    //this player paused
                    return;
                }
            }

            isGamePaused.Value = false;
            //All players are unpaused
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

        #region Is: WaitingToStart

        public bool IsWaitingToStart()
        {
            return state.Value == State.WaitingToStart;
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


        #region Event: OnClientDisconnectCallback

        private void OnClientDisconnectCallback(ulong clientID)
        {
            autoTestGamePausedState = true;
        }

        #endregion


        #region Event: OnStateValueChanged

        private void OnStateValueChanged(State previousvalue, State newvalue)
        {
            OnStateChangedEvent?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Event: OnInteractAction

        private void OnInteractAction(object sender, EventArgs e)
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

        private void OnPauseAction(object sender, EventArgs e)
        {
            TogglePauseGame();
        }

        #endregion

        #region Event: OnPauseValueChanged

        private void OnPauseValueChanged(bool previousvalue, bool newvalue)
        {
            if (isGamePaused.Value)
            {
                Time.timeScale = 0f;
                OnMultiplayerGamePausedEvent?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Time.timeScale = 1f;
                OnMultiplayerGameUnpausedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            GameInput.Instance.OnPauseActionEvent += OnPauseAction;
            GameInput.Instance.OnInteractActionEvent += OnInteractAction;
        }

        private void RemoveEvents()
        {
            GameInput.Instance.OnPauseActionEvent -= OnPauseAction;
            GameInput.Instance.OnInteractActionEvent -= OnInteractAction;
        }

        #endregion
    }
}