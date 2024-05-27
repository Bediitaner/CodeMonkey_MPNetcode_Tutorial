using System;
using System.Collections.Generic;
using Game;
using KitchenChaos_Multiplayer.Game;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LobbyUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Button _btnMainMenu;
        [SerializeField] private Button _btnCreateGame;
        [SerializeField] private Button _btnQuickJoin;
        [SerializeField] private Button _btnJoinCode;
        [SerializeField] private TMP_InputField _ifLobbyCode;
        [SerializeField] private TMP_InputField _ifPlayerName;
        [SerializeField] private LobbyCreateUI _lobbyCreateUI;
        [SerializeField] private Transform _lobbyContainer;
        [SerializeField] private Transform _lobbyTemplate;

        #endregion


        #region Unity: Awake | Start | Update

        private void Awake()
        {
            _lobbyTemplate.gameObject.SetActive(false);
        }

        private void Start()
        {
            AddEvents();

            _ifPlayerName.text = KitchenGameMultiplayer.Instance.GetPlayerName();

            UpdateLobbyList(new List<Lobby>());
        }

        private void OnDestroy()
        {
            RemoveEvents();
        }

        #endregion


        #region Update: Lobby: List

        private void UpdateLobbyList(List<Lobby> lobbyList)
        {
            foreach (Transform child in _lobbyContainer)
            {
                if (child == _lobbyTemplate) continue;
                Destroy(child.gameObject);
            }

            foreach (var lobby in lobbyList)
            {
                var lobbyTransform = Instantiate(_lobbyTemplate, _lobbyContainer);
                lobbyTransform.gameObject.SetActive(true);
                lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
            }
        }

        #endregion


        #region Event: OnBtnCreateGameClicked

        private void OnBtnCreateGameClicked()
        {
            _lobbyCreateUI.Show();
        }

        #endregion

        #region Event: OnBtnQuickJoinClicked

        private void OnBtnQuickJoinClicked()
        {
            KitchenGameLobby.Instance.QuickJoin();
        }

        #endregion

        #region Event: OnBtnJoinCodeClicked

        private void OnBtnJoinCodeClicked()
        {
            KitchenGameLobby.Instance.JoinWithCode(_ifLobbyCode.text);
        }

        #endregion

        #region Event: OnBtnMainMenuClicked

        private void OnBtnMainMenuClicked()
        {
            KitchenGameLobby.Instance.LeaveLobby();
            Loader.Load(Scene.MainMenuScene);
        }

        #endregion

        #region Event: OnPlayerNameChanged

        private void OnPlayerNameChanged(string value)
        {
            KitchenGameMultiplayer.Instance.SetPlayerName(value);
        }

        #endregion

        #region Event: OnLobbyListChanged

        private void OnLobbyListChanged(object sender, OnLobbyListChangedEventArgs e)
        {
            UpdateLobbyList(e.LobbyList);
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            _btnCreateGame.onClick.AddListener(OnBtnCreateGameClicked);
            _btnMainMenu.onClick.AddListener(OnBtnMainMenuClicked);
            _btnQuickJoin.onClick.AddListener(OnBtnQuickJoinClicked);
            _btnJoinCode.onClick.AddListener(OnBtnJoinCodeClicked);

            _ifPlayerName.onValueChanged.AddListener(OnPlayerNameChanged);

            KitchenGameLobby.Instance.OnLobbyListChangedEvent += OnLobbyListChanged;
        }

        private void RemoveEvents()
        {
            _btnCreateGame.onClick.RemoveListener(OnBtnCreateGameClicked);
            _btnMainMenu.onClick.RemoveListener(OnBtnMainMenuClicked);
            _btnQuickJoin.onClick.RemoveListener(OnBtnQuickJoinClicked);
            _btnJoinCode.onClick.RemoveListener(OnBtnJoinCodeClicked);

            _ifPlayerName.onValueChanged.RemoveListener(OnPlayerNameChanged);

            KitchenGameLobby.Instance.OnLobbyListChangedEvent -= OnLobbyListChanged;
        }

        #endregion
    }
}