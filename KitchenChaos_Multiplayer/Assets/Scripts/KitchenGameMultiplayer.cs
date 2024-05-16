using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    #region Singleton

    public static KitchenGameMultiplayer Instance { get; private set; }

    #endregion

    #region Contents

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    #endregion

    #region Unity: Awake

    private void Awake()
    {
        Instance = this;
    }

    #endregion


    #region Start: Host

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (KitchenGameManager.Instance.IsWaitingToStart())
        {
            connectionApprovalResponse.Approved = true;
            connectionApprovalResponse.CreatePlayerObject = true;
        }
        else
        {
            connectionApprovalResponse.Approved = false;
        }
    }

    #endregion

    #region Start: Client

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
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
}