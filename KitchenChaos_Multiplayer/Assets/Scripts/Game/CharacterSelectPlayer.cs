using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace KitchenChaos_Multiplayer.Game
{
    public class CharacterSelectPlayer : MonoBehaviour
    {
        #region Content

        [SerializeField] private int playerIndex;
        [SerializeField] private GameObject goReady;
        [SerializeField] private PlayerVisual playerVisual;
        [SerializeField] private Button btnKick;

        #endregion

        #region Unity: Start

        private void Start()
        {
            AddEvents();

            UpdatePlayer();

            btnKick.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        }

        private void OnDestroy()
        {
            RemoveEvents();
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

                var playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromIndex(playerIndex);
                goReady.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
                playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
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

        #region Event: OnReadyChanged

        private void OnReadyChanged(object sender, EventArgs e)
        {
            UpdatePlayer();
        }

        #endregion

        #region Event: OnKickButtonClicked

        private void OnKickButtonClicked()
        {
            var playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromIndex(playerIndex);
            KitchenGameMultiplayer.Instance.KickPlayer(playerData.clientId);
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChangedEvent += OnPlayerDataNetworkListChanged;
            CharacterSelectReady.Instance.OnReadyChanged += OnReadyChanged;

            btnKick.onClick.AddListener(OnKickButtonClicked);
        }


        private void RemoveEvents()
        {
            KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChangedEvent -= OnPlayerDataNetworkListChanged;
            CharacterSelectReady.Instance.OnReadyChanged -= OnReadyChanged;
            btnKick.onClick.RemoveListener(OnKickButtonClicked);
        }

        #endregion
    }
}