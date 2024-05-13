using System;
using Unity.Netcode;

namespace Counters
{
    public class TrashCounter : BaseCounter
    {
        #region Events

        public static event EventHandler OnAnyObjectTrashed;

        #endregion

        #region Reset: StaticData

        new public static void ResetStaticData()
        {
            OnAnyObjectTrashed = null;
        }

        #endregion


        #region Override: Interact

        public override void Interact(Player player)
        {
            if (player.HasKitchenObject())
            {
                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());

                InteractLogicServerRpc();
            }
        }

        #endregion

        #region ServerRpc: Request: TrashObject

        [ServerRpc(RequireOwnership = false)]
        public void InteractLogicServerRpc()
        {
            InteractLogicClientRpc();
        }

        #endregion

        #region ClientRpc: Request: TrashObject

        [ClientRpc]
        public void InteractLogicClientRpc()
        {
            OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}