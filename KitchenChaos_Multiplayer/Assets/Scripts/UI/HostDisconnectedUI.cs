using System;
using KitchenChaos_Multiplayer.Game;
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

        #region Unity: Awake | Start | OnDestroy

        private void Awake()
        {
            playAgainButton.onClick.AddListener(() => { Loader.Load(Scene.MainMenuScene); });
        }

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


        #region Event: OnClientDisconnectCallback

        private void OnClientDisconnectCallback(ulong clientId)
        {
            Debug.Log("You are kicked.");

            //TODO: FİX BUG
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