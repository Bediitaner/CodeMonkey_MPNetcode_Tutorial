using System;
using UnityEngine;

namespace UI
{
    public class WaitingForOtherPlayersUI : MonoBehaviour
    {
        #region Unity: Start

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

        #region Event: OnLocalPlayerReadyChanged

        private void OnLocalPlayerReadyChanged(object sender, EventArgs e)
        {
            if (KitchenGameManager.Instance.IsLocalPlayerReady())
            {
                Show();
            }
        }

        #endregion

        #region Event: OnStateChanged

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (KitchenGameManager.Instance.IsCountdownToStartActive())
            {
                Hide();
            }
        }

        #endregion
        
        #region Events: Add | Remove

        private void AddEvents()
        {
            KitchenGameManager.Instance.OnLocalPlayerReadyChangedEvent += OnLocalPlayerReadyChanged;
            KitchenGameManager.Instance.OnStateChangedEvent += OnStateChanged;
        }

        private void RemoveEvents()
        {
            KitchenGameManager.Instance.OnStateChangedEvent -= OnStateChanged;
            KitchenGameManager.Instance.OnLocalPlayerReadyChangedEvent -= OnLocalPlayerReadyChanged;
        }

        #endregion
    }
}