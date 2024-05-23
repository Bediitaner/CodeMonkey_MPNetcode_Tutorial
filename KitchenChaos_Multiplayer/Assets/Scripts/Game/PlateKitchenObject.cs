using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace KitchenChaos_Multiplayer.Game
{
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    public class PlateKitchenObject : KitchenObject
    {
        #region Events

        public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAddedEvent;

        #endregion

        #region Contents

        [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

        #endregion

        #region Fields

        private List<KitchenObjectSO> kitchenObjectSOList;

        #endregion

        #region Unity: Awake

        protected override void Awake()
        {
            base.Awake();
            kitchenObjectSOList = new List<KitchenObjectSO>();
        }

        #endregion


        #region Get: KitchenObjectSOList

        public List<KitchenObjectSO> GetKitchenObjectSOList()
        {
            return kitchenObjectSOList;
        }

        #endregion

        #region TryAdd: Ingredient

        public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
        {
            if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
            {
                // Not a valid ingredient
                return false;
            }

            if (kitchenObjectSOList.Contains(kitchenObjectSO))
            {
                // Already has this type
                return false;
            }
            else
            {
                AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));

                return true;
            }
        }

        #endregion

        #region ServerRpc: AddIngredient

        [ServerRpc(RequireOwnership = false)]
        private void AddIngredientServerRpc(int kitchenObjectSOIndex)
        {
            AddIngredientClientRpc(kitchenObjectSOIndex);
        }

        #endregion

        #region ClientRpc: AddIngredient

        [ClientRpc]
        private void AddIngredientClientRpc(int kitchenObjectSOIndex)
        {
            KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        
            kitchenObjectSOList.Add(kitchenObjectSO);

            OnIngredientAddedEvent?.Invoke(this, new OnIngredientAddedEventArgs
            {
                kitchenObjectSO = kitchenObjectSO
            });
        }

        #endregion
    }
}