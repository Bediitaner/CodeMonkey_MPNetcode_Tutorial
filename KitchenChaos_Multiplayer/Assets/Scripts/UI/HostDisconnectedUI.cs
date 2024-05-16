using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HostDisconnectedUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Button playAgainButton;

        #endregion

        #region Unity: Awake | Start

        private void Awake()
        {
            playAgainButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.Shutdown();
                
                Loader.Load(Scene.MainMenuScene);
            });
        }

        private void Start()
        {
            AddEvents();

            Hide();
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


        #region Event: OnClientDisconnectCallback

        private void OnClientDisconnectCallback(ulong clientId)
        {
            if (clientId == NetworkManager.ServerClientId)
            {
                //Server is shutting down
                Show();
            }
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        }

        private void RemoveEvents()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }

        #endregion
    }
}