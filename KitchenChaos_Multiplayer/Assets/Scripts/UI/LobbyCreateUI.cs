using System;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LobbyCreateUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Button _btnCreatePrivate;
        [SerializeField] private Button _btnCreatePublic;
        [SerializeField] private Button _btnHide;
        [SerializeField] private TMP_InputField _ifLobbyName;

        #endregion


        #region Unity: Awake | Start

        private void Awake()
        {
            AddEvents();
        }

        private void Start()
        {
            Hide();
        }

        #endregion


        #region Show | Hide

        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        #endregion
        
        
        #region Event: OnBtnCreatePrivateClicked

        private void OnBtnCreatePrivateClicked()
        {
            KitchenGameLobby.Instance.CreateLobby(_ifLobbyName.text,true);
        }

        #endregion

        #region Event: OnBtnCreatePublicClicked

        private void OnBtnCreatePublicClicked()
        {
            KitchenGameLobby.Instance.CreateLobby(_ifLobbyName.text,false);
        }

        #endregion

        #region Event: OnBtnHideClicked

        private void OnBtnHideClicked()
        {
            Hide();
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            _btnCreatePrivate.onClick.AddListener(OnBtnCreatePrivateClicked);
            _btnCreatePrivate.onClick.AddListener(OnBtnCreatePublicClicked);
            _btnHide.onClick.AddListener(OnBtnHideClicked);
        }

        private void RemoveEvents()
        {
            _btnCreatePrivate.onClick.RemoveListener(OnBtnCreatePrivateClicked);
            _btnCreatePrivate.onClick.RemoveListener(OnBtnCreatePublicClicked);
            _btnHide.onClick.RemoveListener(OnBtnHideClicked);
        }

        #endregion
    }
}