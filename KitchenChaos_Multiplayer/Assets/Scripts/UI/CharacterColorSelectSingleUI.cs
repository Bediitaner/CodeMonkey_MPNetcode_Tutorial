using System;
using KitchenChaos_Multiplayer.Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CharacterColorSelectSingleUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private int colorId;
        [SerializeField] private Image image;
        [SerializeField] private GameObject selectedGameObject;
        [SerializeField] private Button btnSelect;

        #endregion

        #region Unity: Awake| Start | OnDestroy

        private void Awake()
        {
            AddEvents();
        }

        private void Start()
        {
            KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChangedEvent += OnPlayerDataNetworkListChanged;
            image.color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);
            UpdateIsSelected();
        }

        private void OnDestroy()
        {
            KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChangedEvent -= OnPlayerDataNetworkListChanged;
        }

        #endregion

        #region Update: IsSelected

        public void UpdateIsSelected()
        {
            selectedGameObject.gameObject.SetActive(KitchenGameMultiplayer.Instance.GetPlayerData().colorId == colorId);
        }

        #endregion

        #region Event: OnSelectButtonClicked

        private void OnSelectButtonClicked()
        {
            KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
        }

        #endregion

        #region Event: OnPlayerDataNetworkListChanged

        private void OnPlayerDataNetworkListChanged(object sender, EventArgs e)
        {
            UpdateIsSelected();
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            btnSelect.onClick.AddListener(OnSelectButtonClicked);
        }

        private void RemoveEvents()
        {
            btnSelect.onClick.RemoveListener(OnSelectButtonClicked);
        }

        #endregion
    }
}