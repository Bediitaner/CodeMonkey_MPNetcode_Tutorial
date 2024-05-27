using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KitchenChaos_Multiplayer.Game;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Scene = KitchenChaos_Multiplayer.Game.Scene;

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

        private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

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


        #region Async: Allocate: Relay

        private async Task<Allocation> AllocateRelay()
        {
            try
            {
                var allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_PLAYER_AMOUNT - 1);
                return allocation;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return default;
            }
        }

        #endregion


        #region Async: Get: Relay: JoinCode

        private async Task<String> GetRelayJoinCode(Allocation allocation)
        {
            try
            {
                var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                return relayJoinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return default;
            }
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

                var allocation = await AllocateRelay();
                var relayJoinCode = await GetRelayJoinCode(allocation);

                await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)
                        }
                    }
                });

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

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

        #region Async: Quick: Join: Lobby

        public async void QuickJoin()
        {
            OnJoinStartedEvent?.Invoke(this, EventArgs.Empty);

            try
            {
                _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

                var relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
                var joinAllocation = await JoinRelay(relayJoinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

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

                var relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
                var joinAllocation = await JoinRelay(relayJoinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

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

                var relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
                var joinAllocation = await JoinRelay(relayJoinCode);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

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


        #region Join: Relay

        private async Task<JoinAllocation> JoinRelay(string relayJoinCode)
        {
            try
            {
                var joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
                return joinAllocation;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return default;
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
            if (_joinedLobby == null && AuthenticationService.Instance.IsSignedIn && SceneManager.GetActiveScene().name == Scene.LobbyScene.ToString())
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