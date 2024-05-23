using System;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos_Multiplayer.Game
{
    public class PlateCompleteVisual : MonoBehaviour
    {
        [Serializable]
        public struct KitchenObjectSO_GameObject
        {
            public KitchenObjectSO kitchenObjectSO;
            public GameObject gameObject;
        }


        #region Contents

        [SerializeField] private PlateKitchenObject plateKitchenObject;
        [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSOGameObjectList;
    
        #endregion

        #region Unity: Start

        private void Start()
        {
            AddEvents();
        
            foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
            {
                kitchenObjectSOGameObject.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Event: OnIngredientAdded

        private void OnIngredientAdded(object sender, OnIngredientAddedEventArgs e)
        {
            foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
            {
                if (kitchenObjectSOGameObject.kitchenObjectSO == e.kitchenObjectSO)
                {
                    kitchenObjectSOGameObject.gameObject.SetActive(true);
                }
            }
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            plateKitchenObject.OnIngredientAddedEvent += OnIngredientAdded;
        }

        private void RemoveEvents()
        {

        }

        #endregion
    }
}