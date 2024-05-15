using System;
using UnityEngine;

namespace UI
{
    public class PauseMultiplayerUI : MonoBehaviour
    {
        #region Unity: Awake | Start

        private void Awake()
        {
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

        #region Event: OnMultiplayerGamePaused

        private void OnMultiplayerGamePaused(object sender, EventArgs e)
        {
            Show();
        }

        #endregion

        #region Event: OnMultiplayerGameUnpaused

        private void OnMultiplayerGameUnpaused(object sender, EventArgs e)
        {
            Hide();
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            KitchenGameManager.Instance.OnMultiplayerGamePausedEvent += OnMultiplayerGamePaused;
            KitchenGameManager.Instance.OnMultiplayerGameUnpausedEvent += OnMultiplayerGameUnpaused;
        }

        private void RemoveEvents()
        {
            KitchenGameManager.Instance.OnMultiplayerGamePausedEvent -= OnMultiplayerGamePaused;
            KitchenGameManager.Instance.OnMultiplayerGameUnpausedEvent -= OnMultiplayerGameUnpaused;
        }

        #endregion
    }
}