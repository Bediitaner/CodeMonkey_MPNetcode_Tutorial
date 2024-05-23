using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos_Multiplayer.Game
{
    public class CharacterSelectReady : NetworkBehaviour
    {
        #region Singleton

        public static CharacterSelectReady Instance { get; private set; }

        #endregion

        #region Events

        public event EventHandler OnReadyChanged;

        #endregion

        #region Fields

        private Dictionary<ulong, bool> playerReadyDictionary;

        #endregion


        #region Unity: Awake

        private void Awake()
        {
            Instance = this;

            playerReadyDictionary = new Dictionary<ulong, bool>();
        }

        #endregion


        #region Set: PlayerReady

        public void SetPlayerReady()
        {
            SetPlayerReadyServerRpc();
        }

        #endregion


        #region ServerRpc: Set: PlayerReady

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
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
                Loader.LoadNetwork(Scene.GameScene);
            }

            Debug.Log("allClientsReady: " + allClientsReady);
        }

        #endregion

        #region ClientRpc: Set: PlayerReady

        [ClientRpc]
        private void SetPlayerReadyClientRpc(ulong clientId)
        {
            playerReadyDictionary[clientId] = true;

            OnReadyChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Is: Player: Ready

        public bool IsPlayerReady(ulong clientId)
        {
           return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
        }

        #endregion
    }
}