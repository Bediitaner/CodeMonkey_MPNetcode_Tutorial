using UnityEngine;

namespace UI
{
    public class PlateIconsUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private PlateKitchenObject plateKitchenObject;
        [SerializeField] private Transform iconTemplate;

        #endregion
        
        #region Unity: Awake | Start

        private void Awake()
        {
            iconTemplate.gameObject.SetActive(false);
        }

        private void Start()
        {
        }

        #endregion
        
        
        #region Update: UI

        private void UpdateUI()
        {
            foreach (Transform child in transform)
            {
                if (child == iconTemplate) continue;
                Destroy(child.gameObject);
            }

            foreach (KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
            {
                Transform iconTransform = Instantiate(iconTemplate, transform);
                iconTransform.gameObject.SetActive(true);
                iconTransform.GetComponent<PlateIconsSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
            }
        }
        

        #endregion
        
        
        #region Event: OnIngredientAdded

        private void IngredientAddedEvent(object sender, OnIngredientAddedEventArgs e)
        {
            UpdateUI();
        }

        #endregion
        
        #region Events: Add | Remove

        private void AddEvents()
        {
            plateKitchenObject.OnIngredientAddedEvent += IngredientAddedEvent;
        }

        private void RemoveEvents()
        {
            plateKitchenObject.OnIngredientAddedEvent -= IngredientAddedEvent;
        }

        #endregion
    }
}