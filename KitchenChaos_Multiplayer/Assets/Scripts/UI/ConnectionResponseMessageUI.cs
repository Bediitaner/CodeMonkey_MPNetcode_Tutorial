using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ConnectionResponseMessageUI : MonoBehaviour
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


        #region Event: OnCloseClicked

        private void OnCloseClicked()
        {
            Hide();
        }

        #endregion

        #region Event: OnFailedToJoinGame

        private void OnFailedToJoinGame(object sender, EventArgs e)
        {
            Show();

            _txtMessage.text = NetworkManager.Singleton.DisconnectReason;

            if (_txtMessage.text == "")
            {
                _txtMessage.text = "Failed to connect.";
            }
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            _btnClose.onClick.AddListener(OnCloseClicked);

            KitchenGameMultiplayer.Instance.OnFailedToJoinGameEvent += OnFailedToJoinGame;
        }

        private void RemoveEvents()
        {
            _btnClose.onClick.RemoveListener(OnCloseClicked);

            KitchenGameMultiplayer.Instance.OnFailedToJoinGameEvent -= OnFailedToJoinGame;
        }

        #endregion
    }
}