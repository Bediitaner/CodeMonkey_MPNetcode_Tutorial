using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TestingCharacterSelectUI : MonoBehaviour
    {
        [SerializeField] private Button _btnReady;

        private void Awake()
        {
            AddEvents();
        }

        #region Event: OnReadyClicked

        private void OnReadyClicked()
        {
            CharacterSelectReady.Instance.SetPlayerReady();
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            _btnReady.onClick.AddListener(OnReadyClicked);
        }

        private void RemoveEvents()
        {
            _btnReady.onClick.RemoveListener(OnReadyClicked);
        }

        #endregion
    }
}