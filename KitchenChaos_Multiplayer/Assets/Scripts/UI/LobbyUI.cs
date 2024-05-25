using System;
using Game;
using KitchenChaos_Multiplayer.Game;
using TMPro;
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

        #endregion

        
        #region Unity: Awake | Start | Update

        private void Awake()
        {
            AddEvents();
        }

        private void Start()
        {
            _ifPlayerName.text = KitchenGameMultiplayer.Instance.GetPlayerName();
        }

        private void OnDestroy()
        {
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
            Loader.Load(Scene.MainMenuScene);
        }

        #endregion

        #region Event: OnPlayerNameChanged

        private void OnPlayerNameChanged(string value)
        {
            KitchenGameMultiplayer.Instance.SetPlayerName(value);
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
        }

        private void RemoveEvents()
        {
            _btnCreateGame.onClick.RemoveListener(OnBtnCreateGameClicked);
            _btnMainMenu.onClick.RemoveListener(OnBtnMainMenuClicked);
            _btnQuickJoin.onClick.RemoveListener(OnBtnQuickJoinClicked);
            _btnJoinCode.onClick.RemoveListener(OnBtnJoinCodeClicked);
            
            _ifPlayerName.onValueChanged.RemoveListener(OnPlayerNameChanged);
        }

        #endregion
    }
}