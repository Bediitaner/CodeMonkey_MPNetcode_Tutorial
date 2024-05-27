using System;
using System.Collections.Generic;
using KitchenChaos_Multiplayer.Game;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class KitchenGameLobby : MonoBehaviour
    {
        #region Singleton

        public static KitchenGameLobby Instance { get; private set; }

        #endregion

        #region Events

        public event EventHandler OnCreateLobbyStartedEvent;
        public event EventHandler OnCreateLobbyFailedEvent;
        public event EventHandler OnJoinStartedEvent;
        public event EventHandler OnQuickJoinFailedEvent;
        public event EventHandler OnJoinFailedEvent;
        public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChangedEvent;

        #endregion

        #region Fields

        private Lobby _joinedLobby;
        private float heartbeatTimer;
        private float listLobbiesTimer;

        #endregion

        #region Unity: Awake | Update

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            InitializeUnityAuthentication();
        }

        private void Update()
        {
            HandleHeartbeat();
            HandlePeriodicListLobbies();
        }

        #endregion


        #region Async: Init: Unity: Authentication

        private async void InitializeUnityAuthentication()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                var initializationOptions = new InitializationOptions();

                initializationOptions.SetProfile(Random.RandomRange(0, 1000).ToString());

                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        #endregion

        #region Async: Create: Lobby

        public async void CreateLobby(string lobbyName, bool isPrivate)
        {
            OnCreateLobbyStartedEvent?.Invoke(this, EventArgs.Empty);

            try
            {
                _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions
                {
                    IsPrivate = isPrivate
                });

                KitchenGameMultiplayer.Instance.StartHost();
                Loader.LoadNetwork(Scene.CharacterSelectScene);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                OnCreateLobbyFailedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Async: Join: Lobby

        public async void QuickJoin()
        {
            OnJoinStartedEvent?.Invoke(this, EventArgs.Empty);

            try
            {
                _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

                KitchenGameMultiplayer.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                OnQuickJoinFailedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Async: Join: Lobby: Code

        public async void JoinWithCode(string lobbyCode)
        {
            OnJoinStartedEvent?.Invoke(this, EventArgs.Empty);
            try
            {
                _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

                KitchenGameMultiplayer.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                OnJoinFailedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Async: Join: Lobby: Id

        public async void JoinWithId(string lobbyId)
        {
            OnJoinStartedEvent?.Invoke(this, EventArgs.Empty);
            try
            {
                _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

                KitchenGameMultiplayer.Instance.StartClient();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                OnJoinFailedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Async: Delete: Lobby

        public async void DeleteLobby()
        {
            if (_joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
                    _joinedLobby = null;
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }

        #endregion

        #region Async: Leave: Lobby

        public async void LeaveLobby()
        {
            if (_joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }

        #endregion

        #region Async: Kick: Player

        public async void KickPlayer(string playerId)
        {
            if (IsLobbyHost())
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }

        #endregion


        #region Get: Lobby

        public Lobby GetLobby()
        {
            return _joinedLobby;
        }

        #endregion

        #region Is: Lobby: Host

        private bool IsLobbyHost()
        {
            return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        #endregion


        #region List: Lobbies

        private async void ListLobbies()
        {
            try
            {
                var queryLobbyOptions = new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter>()
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    }
                };
                QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbyOptions);

                OnLobbyListChangedEvent?.Invoke(this, new OnLobbyListChangedEventArgs
                {
                    LobbyList = queryResponse.Results
                });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        #endregion


        #region Handle: Heartbeat

        private void HandleHeartbeat()
        {
            if (IsLobbyHost())
            {
                heartbeatTimer -= Time.deltaTime;

                if (heartbeatTimer <= 0f)
                {
                    var heartbeatTimerMax = 15f;
                    heartbeatTimer = heartbeatTimerMax;

                    LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
                }
            }
        }

        #endregion

        #region Handle: Periodic: List: Lobbies

        private void HandlePeriodicListLobbies()
        {
            if (_joinedLobby != null && AuthenticationService.Instance.IsSignedIn)
            {
                listLobbiesTimer -= Time.deltaTime;

                if (listLobbiesTimer <= 0f)
                {
                    var heartbeatTimerMax = 3f;
                    listLobbiesTimer = heartbeatTimerMax;

                    ListLobbies();
                }
            }
        }

        #endregion
    }
}