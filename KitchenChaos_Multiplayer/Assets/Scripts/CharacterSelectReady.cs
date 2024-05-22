using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    #region Singleton

    public static CharacterSelectReady Instance { get; private set; }

    #endregion
    
    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
        
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    #region Set: PlayerReady

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
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
            Loader.LoadNetwork(Scene.GameScene);
        }

        Debug.Log("allClientsReady: " + allClientsReady);
    }

    #endregion
}
