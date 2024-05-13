using System;
using Unity.Netcode;
using UnityEngine;

namespace Counters
{
    public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
    {
        #region Events

        public static event EventHandler OnAnyObjectPlacedHere;

        #endregion

        #region Contents

        [SerializeField] private Transform counterTopPoint;

        #endregion

        #region Fields

        private KitchenObject kitchenObject;

        #endregion


        #region Reset: StaticData

        public static void ResetStaticData()
        {
            OnAnyObjectPlacedHere = null;
        }

        #endregion


        #region Interact

        public virtual void Interact(Player player)
        {
            Debug.LogError("BaseCounter.Interact();");
        }

        #endregion

        #region Interact: Alternate

        public virtual void InteractAlternate(Player player)
        {
            //Debug.LogError("BaseCounter.InteractAlternate();");
        }

        #endregion


        #region KitchenObject: GetFollowTransform

        public Transform GetKitchenObjectFollowTransform()
        {
            return counterTopPoint;
        }

        #endregion

        #region KitchenObject: Set

        public void SetKitchenObject(KitchenObject kitchenObject)
        {
            this.kitchenObject = kitchenObject;

            if (kitchenObject != null)
            {
                OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region KitchenObject: Get

        public KitchenObject GetKitchenObject()
        {
            return kitchenObject;
        }

        #endregion

        #region KitchenObject: Clear

        public void ClearKitchenObject()
        {
            kitchenObject = null;
        }

        #endregion

        #region KitchenObject: Has

        public bool HasKitchenObject()
        {
            return kitchenObject != null;
        }

        #endregion


        #region Get: NetworkObject

        public NetworkObject GetNetworkObject()
        {
            return NetworkObject;
        }

        #endregion
    }
}