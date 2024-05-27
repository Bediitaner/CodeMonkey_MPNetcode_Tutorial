using System;
using Game;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LobbyListSingleUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Button _btnJoin;
        [SerializeField] private TextMeshProUGUI _txtLobbyName;

        #endregion

        #region Fields

        private Lobby _lobby;

        #endregion


        #region Unity: Awake 

        private void Awake()
        {
            AddEvents();
        }

        private void OnDestroy()
        {
            RemoveEvents();
        }

        #endregion

        
        #region Set: Lobby

        public void SetLobby(Lobby lobby)
        {
            _lobby = lobby;
        }

        #endregion

        
        #region Event: OnBtnJoinClicked

        private void OnBtnJoinClicked()
        {
            KitchenGameLobby.Instance.JoinWithId(_lobby.Id);
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            _btnJoin.onClick.AddListener(OnBtnJoinClicked);
        }

        private void RemoveEvents()
        {
            _btnJoin.onClick.RemoveListener(OnBtnJoinClicked);
        }

        #endregion
    }
}