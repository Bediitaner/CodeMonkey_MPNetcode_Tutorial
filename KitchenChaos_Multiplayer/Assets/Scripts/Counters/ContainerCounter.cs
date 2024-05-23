using System;
using KitchenChaos_Multiplayer.Game;
using Unity.Netcode;
using UnityEngine;

namespace Counters
{
    public class ContainerCounter : BaseCounter
    {
        #region Events

        public event EventHandler OnPlayerGrabbedObjectEvent;

        #endregion


        #region Contents

        [SerializeField] private KitchenObjectSO kitchenObjectSO;

        #endregion

        
        #region Override: Interact

        public override void Interact(Player player)
        {
            if (!player.HasKitchenObject())
            {
                // Player is not carrying anything
                KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
                InteractLogicServerRpc();
            }
        }

        #endregion

        #region ServerRpc: InteractLogic

        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicServerRpc()
        {
            InteractLogicClientRpc();
        }

        #endregion

        #region ClientRpc: InteractLogic

        [ClientRpc]
        private void InteractLogicClientRpc()
        {
            OnPlayerGrabbedObjectEvent?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}