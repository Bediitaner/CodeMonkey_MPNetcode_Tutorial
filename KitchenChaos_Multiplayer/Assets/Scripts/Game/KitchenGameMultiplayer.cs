using System;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace KitchenChaos_Multiplayer.Game
{
    public class KitchenGameMultiplayer : NetworkBehaviour
    {
        #region Singleton

        public static KitchenGameMultiplayer Instance { get; private set; }

        #endregion

        #region Events

        public event EventHandler OnTryingToJoinGameEvent;
        public event EventHandler OnFailedToJoinGameEvent;
        public event EventHandler OnPlayerDataNetworkListChangedEvent;

        #endregion

        #region Contents

        [SerializeField] private KitchenObjectListSO kitchenObjectListSO;
        [SerializeField] private List<Color> playerColorList;

        #endregion

        #region Fields

        public const int MAX_PLAYER_AMOUNT = 4;
        public const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

        private NetworkList<PlayerData> playerDataNetworkList;
        private string _playerName;

        #endregion

        #region Unity: Awake

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            _playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + Random.Range(100, 1000));

            playerDataNetworkList = new NetworkList<PlayerData>();
            playerDataNetworkList.OnListChanged += OnPlayerDataNetworkListChanged;
        }

        #endregion


        #region Start: Host | OnClientConnected | ApprovalCheck

        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
            NetworkManager.Singleton.StartHost();
        }

        private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
        {
            for (int i = 0; i < playerDataNetworkList.Count; i++)
            {
                var playerData = playerDataNetworkList[i];
                if (playerData.clientId == clientId)
                {
                    playerDataNetworkList.RemoveAt(i);
                }
            }
        }

        private void NetworkManager_OnClientConnected(ulong clientId)
        {
            playerDataNetworkList.Add(new PlayerData
            {
                clientId = clientId,
                colorId = GetFirstUnusedColorId(),
            });
            
            SetPlayerNameServerRpc(GetPlayerName());
        }

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,
            NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
        {
            if (SceneManager.GetActiveScene().name != Scene.CharacterSelectScene.ToString())
            {
                connectionApprovalResponse.Approved = false;
                connectionApprovalResponse.Reason = "Game has already started";
                return;
            }

            if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
            {
                connectionApprovalResponse.Approved = false;
                connectionApprovalResponse.Reason = "Game is full";
                return;
            }

            connectionApprovalResponse.Approved = true;
        }

        #endregion

        #region Start: Client

        public void StartClient()
        {
            OnTryingToJoinGameEvent?.Invoke(this, EventArgs.Empty);

            NetworkManager.Singleton.OnClientDisconnectCallback += Client_OnClientDisconnectCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += Client_OnClientConnectedCallback;
            NetworkManager.Singleton.StartClient();
        }

        private void Client_OnClientConnectedCallback(ulong obj)
        {
            SetPlayerNameServerRpc(GetPlayerName());
        }

        private void Client_OnClientDisconnectCallback(ulong obj)
        {
            OnFailedToJoinGameEvent?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Spawn: KitchenObject

        public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
        {
            SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
        }

        #endregion


        #region ServerRpc: Spawn: KitchenObject

        [ServerRpc(RequireOwnership = false)]
        private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
        {
            KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
            Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

            NetworkObject kitchenNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
            kitchenNetworkObject.Spawn(true);

            KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

            kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
            IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

            kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        }

        #endregion


        #region Get: KitchenObjectSO: Index

        public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
        {
            return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
        }

        #endregion

        #region Get: KitchenObjectSO: From: Index

        public KitchenObjectSO GetKitchenObjectSOFromIndex(int index)
        {
            return kitchenObjectListSO.kitchenObjectSOList[index];
        }

        #endregion

        #region Get: PlayerData: From: Index

        public PlayerData GetPlayerDataFromIndex(int playerIndex)
        {
            return playerDataNetworkList[playerIndex];
        }

        #endregion

        #region Get: PlayerData: From: ClientId

        public PlayerData GetPlayerDataFromClientId(ulong clientId)
        {
            foreach (var playerData in playerDataNetworkList)
            {
                if (playerData.clientId == clientId)
                {
                    return playerData;
                }
            }

            return default;
        }

        #endregion

        #region Get: PlayerData: Index: From: ClientId

        public int GetPlayerDataIndexFromClientId(ulong clientId)
        {
            for (int i = 0; i < playerDataNetworkList.Count; i++)
            {
                if (playerDataNetworkList[i].clientId == clientId)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region Get: PlayerData

        public PlayerData GetPlayerData()
        {
            return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
        }

        #endregion

        #region Get: Player: Color

        public Color GetPlayerColor(int colorId)
        {
            return playerColorList[colorId];
        }

        #endregion

        #region Get: First: Unused: Color: Id

        private int GetFirstUnusedColorId()
        {
            for (int i = 0; i < playerColorList.Count; i++)
            {
                if (IsColorAvailable(i))
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region Get: PlayerName

        public string GetPlayerName()
        {
            return _playerName;
        }

        #endregion


        #region Set: PlayerName

        public void SetPlayerName(string playerName)
        {
            this._playerName = playerName;
            PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
        }

        #endregion


        #region Change: Player: Color

        public void ChangePlayerColor(int colorId)
        {
            ChangePlayerColorServerRpc(colorId);
        }

        #endregion

        #region ServerRpc: Change: Player: Color

        [ServerRpc(RequireOwnership = false)]
        private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
        {
            if (!IsColorAvailable(colorId))
            {
                return;
            }

            int playerIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            PlayerData playerData = playerDataNetworkList[playerIndex];
            playerData.colorId = colorId;
            playerDataNetworkList[playerIndex] = playerData;
        }

        #endregion

        #region ServerRpc: Set: PlayerName

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
        {
            int playerIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
            PlayerData playerData = playerDataNetworkList[playerIndex];
            playerData.playerName = playerName;
            playerDataNetworkList[playerIndex] = playerData;
        }

        #endregion


        #region Is: Color: Available

        private bool IsColorAvailable(int colorId)
        {
            foreach (var playerData in playerDataNetworkList)
            {
                if (playerData.colorId == colorId)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion


        #region Destroy: KitchenObject

        public void DestroyKitchenObject(KitchenObject kitchenObject)
        {
            DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
        }

        #endregion

        #region ServerRpc: Destroy: KitchenObject

        [ServerRpc(RequireOwnership = false)]
        private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
        {
            kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
            KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

            ClearKitchenObjectOnParentClientRpc(kitchenObject.NetworkObject);

            kitchenObject.DestroySelf();
        }

        #endregion

        #region ClientRpc: Clear: KitchenObject

        [ClientRpc]
        private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
        {
            kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
            KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

            kitchenObject.ClearKitchenObjectParent();
        }

        #endregion


        #region Is: Player: Index: Connected

        public bool IsPlayerIndexConnected(int playerIndex)
        {
            return playerIndex < playerDataNetworkList.Count;
        }

        #endregion

        #region Kick: Player

        public void KickPlayer(ulong clientId)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            NetworkManager_Server_OnClientDisconnectCallback(clientId);
        }

        #endregion


        #region Event: OnPlayerDataNetworkListChanged

        private void OnPlayerDataNetworkListChanged(NetworkListEvent<PlayerData> changeevent)
        {
            OnPlayerDataNetworkListChangedEvent?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}