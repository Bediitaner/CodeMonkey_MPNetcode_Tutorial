using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlateIconsSingleUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Image image;

        #endregion

        #region Set: KitchenObjectSO

        public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
        {
            image.sprite = kitchenObjectSO.sprite;
        }

        #endregion
    }
}