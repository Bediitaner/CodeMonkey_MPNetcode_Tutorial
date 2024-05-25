using System;
using Game;
using KitchenChaos_Multiplayer.Game;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CharacterSelectUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Button _btnMainMenu;
        [SerializeField] private Button _btnReady;
        [SerializeField] private TextMeshProUGUI _txtLobbyName;
        [SerializeField] private TextMeshProUGUI _txtLobbyCode;

        #endregion

        #region Unity: Awake | Start

        private void Awake()
        {
            AddEvents();
        }

        private void Start()
        {
            var lobby = KitchenGameLobby.Instance.GetLobby();
            _txtLobbyName.text = "Lobby Name:" + lobby.Name;
            _txtLobbyCode.text = "Lobby Code:" + lobby.LobbyCode;
        }

        #endregion


        #region Event: OnBtnMainMenuClicked

        private void OnBtnMainMenuClicked()
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Scene.MainMenuScene);
        }

        #endregion

        #region Event: OnBtnReadyClicked

        private void OnBtnReadyClicked()
        {
            CharacterSelectReady.Instance.SetPlayerReady();
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            _btnMainMenu.onClick.AddListener(OnBtnMainMenuClicked);
            _btnReady.onClick.AddListener(OnBtnReadyClicked);
        }

        private void RemoveEvents()
        {
            _btnMainMenu.onClick.RemoveListener(OnBtnMainMenuClicked);
            _btnReady.onClick.RemoveListener(OnBtnReadyClicked);
        }

        #endregion
    }
}