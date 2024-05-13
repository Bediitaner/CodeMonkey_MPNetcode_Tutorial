using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DeliveryManagerSingleUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private TextMeshProUGUI recipeNameText;
        [SerializeField] private Transform iconContainer;
        [SerializeField] private Transform iconTemplate;

        #endregion

        #region Unity: Awake

        private void Awake()
        {
            iconTemplate.gameObject.SetActive(false);
        }

        #endregion

        #region Set: RecipeSO

        public void SetRecipeSO(RecipeSO recipeSO)
        {
            recipeNameText.text = recipeSO.recipeName;

            foreach (Transform child in iconContainer)
            {
                if (child == iconTemplate) continue;
                Destroy(child.gameObject);
            }

            foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList)
            {
                Transform iconTransform = Instantiate(iconTemplate, iconContainer);
                iconTransform.gameObject.SetActive(true);
                iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
            }
        }

        #endregion
    }
}