using System;
using UnityEngine;

namespace UI
{
    public class DeliveryManagerUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Transform container;
        [SerializeField] private Transform recipeTemplate;

        #endregion

        #region Unity: Awake | Start

        private void Awake()
        {
            recipeTemplate.gameObject.SetActive(false);
        }

        private void Start()
        {
            AddEvents();

            UpdateUI();
        }

        #endregion


        #region Update: UI

        private void UpdateUI()
        {
            foreach (Transform child in container)
            {
                if (child == recipeTemplate) continue;
                Destroy(child.gameObject);
            }

            foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList())
            {
                Transform recipeTransform = Instantiate(recipeTemplate, container);
                recipeTransform.gameObject.SetActive(true);
                recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
            }
        }

        #endregion

        
        #region Event: OnRecipeCompleted

        private void RecipeCompletedEvent(object sender, EventArgs e)
        {
            UpdateUI();
        }

        #endregion

        #region Event: OnRecipeSpawned

        private void RecipeSpawnedEvent(object sender, EventArgs e)
        {
            UpdateUI();
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            DeliveryManager.Instance.OnRecipeSpawnedEvent += RecipeSpawnedEvent;
            DeliveryManager.Instance.OnRecipeCompletedEvent += RecipeCompletedEvent;
        }
        
        private void RemoveEvents()
        {
            DeliveryManager.Instance.OnRecipeSpawnedEvent -= RecipeSpawnedEvent;
            DeliveryManager.Instance.OnRecipeCompletedEvent -= RecipeCompletedEvent;
        }

        #endregion
    }
}