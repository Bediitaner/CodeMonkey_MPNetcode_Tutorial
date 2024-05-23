using System;
using KitchenChaos_Multiplayer.Game;
using UnityEngine;

namespace UI
{
    public class ConnectingUI : MonoBehaviour
    {
        #region Unity: Start

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


        #region Event: OnTryingToJoinGame

        private void OnTryingToJoinGame(object sender, EventArgs e)
        {
            Show();
        }

        #endregion

        #region Event: OnFailedToJoinGame

        private void OnFailedToJoinGame(object sender, EventArgs e)
        {
            Hide();
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            KitchenGameMultiplayer.Instance.OnTryingToJoinGameEvent += OnTryingToJoinGame;
            KitchenGameMultiplayer.Instance.OnFailedToJoinGameEvent += OnFailedToJoinGame;
        }


        private void RemoveEvents()
        {
            KitchenGameMultiplayer.Instance.OnTryingToJoinGameEvent -= OnTryingToJoinGame;
            KitchenGameMultiplayer.Instance.OnFailedToJoinGameEvent -= OnFailedToJoinGame;
        }

        #endregion
    }
}