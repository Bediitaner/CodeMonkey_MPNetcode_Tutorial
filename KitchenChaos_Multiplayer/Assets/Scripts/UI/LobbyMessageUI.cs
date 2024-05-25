using System;
using Game;
using KitchenChaos_Multiplayer.Game;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LobbyMessageUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Button _btnClose;
        [SerializeField] private TextMeshProUGUI _txtMessage;

        #endregion

        #region Unity: Awake

        private void Start()
        {
            AddEvents();
            Hide();
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


        #region

        private void ShowMessage(string message)
        {
            Show();
            _txtMessage.text = message;
        }

        #endregion


        #region Event: OnCloseClicked

        private void OnCloseClicked()
        {
            Hide();
        }

        #endregion

        #region Event: OnFailedToJoinGame

        private void OnFailedToJoinGame(object sender, EventArgs e)
        {
            if (NetworkManager.Singleton.DisconnectReason == "")
            {
                ShowMessage("Failed to connect.");
            }
            else
            {
                ShowMessage(NetworkManager.Singleton.DisconnectReason);
            }
        }

        #endregion

        #region Event: OnCreateLobbyStarted

        private void OnCreateLobbyStarted(object sender, EventArgs e)
        {
            ShowMessage("Creating Lobby...");
        }

        #endregion

        #region Event: OnCreateLobbyFailed

        private void OnCreateLobbyFailed(object sender, EventArgs e)
        {
            ShowMessage("Failed to creating lobby!");
        }

        #endregion

        #region Event: OnJoinStarted
        
        private void OnJoinStarted(object sender, EventArgs e)
        {
            ShowMessage("Joining Lobby...");
        }

        #endregion

        #region Event: OnJoinFailed

        private void OnJoinFailed(object sender, EventArgs e)
        {
            ShowMessage("Failed to join lobby!");
        }

        #endregion

        #region Event: OnQuickJoinFailed

        private void OnQuickJoinFailed(object sender, EventArgs e)
        {
            ShowMessage("Could not find a lobby to join.");
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            _btnClose.onClick.AddListener(OnCloseClicked);

            KitchenGameMultiplayer.Instance.OnFailedToJoinGameEvent += OnFailedToJoinGame;
            KitchenGameLobby.Instance.OnCreateLobbyFailedEvent += OnCreateLobbyFailed;
            KitchenGameLobby.Instance.OnCreateLobbyStartedEvent += OnCreateLobbyStarted;
            KitchenGameLobby.Instance.OnJoinStartedEvent += OnJoinStarted;
            KitchenGameLobby.Instance.OnJoinFailedEvent += OnJoinFailed;
            KitchenGameLobby.Instance.OnQuickJoinFailedEvent += OnQuickJoinFailed;
        }

        private void RemoveEvents()
        {
            _btnClose.onClick.RemoveListener(OnCloseClicked);

            KitchenGameMultiplayer.Instance.OnFailedToJoinGameEvent -= OnFailedToJoinGame;
            KitchenGameLobby.Instance.OnCreateLobbyFailedEvent -= OnCreateLobbyFailed;
            KitchenGameLobby.Instance.OnCreateLobbyStartedEvent -= OnCreateLobbyStarted;
            KitchenGameLobby.Instance.OnJoinStartedEvent -= OnJoinStarted;
            KitchenGameLobby.Instance.OnJoinFailedEvent -= OnJoinFailed;
            KitchenGameLobby.Instance.OnQuickJoinFailedEvent -= OnQuickJoinFailed;
        }

        #endregion
    }
}