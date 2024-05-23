using System;
using UnityEngine;

namespace KitchenChaos_Multiplayer.Game
{
    public class CharacterSelectPlayer : MonoBehaviour
    {
        #region Content

        [SerializeField] private int playerIndex;

        #endregion

        #region Unity: Start

        private void Start()
        {
            AddEvents();

            UpdatePlayer();
        }

        #endregion

        #region Show | Hide

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region Update: Player

        private void UpdatePlayer()
        {
            if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        #endregion


        #region Event: OnPlayerDataNetworkListChanged

        private void OnPlayerDataNetworkListChanged(object sender, EventArgs e)
        {
            UpdatePlayer();
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChangedEvent += OnPlayerDataNetworkListChanged;
        }


        private void RemoveEvents()
        {
            KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChangedEvent -= OnPlayerDataNetworkListChanged;
        }

        #endregion
    }
}