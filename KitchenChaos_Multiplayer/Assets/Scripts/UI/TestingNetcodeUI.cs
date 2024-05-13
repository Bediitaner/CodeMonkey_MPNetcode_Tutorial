using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TestingNetcodeUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;

        #endregion

        #region Unity: Awake

        private void Awake()
        {
            AddEvents();
        }

        #endregion

        
        #region Hide

        private void Hide()
        {
            gameObject.SetActive(false);
        }
        

        #endregion
        
        
        #region Event: OnHostButtonClicked

        private void OnHostButtonClicked()
        {
            Debug.Log("Host");
            NetworkManager.Singleton.StartHost();
            Hide();
        }

        #endregion

        #region Event: OnClientButtonClicked

        private void OnClientButtonClicked()
        {
            Debug.Log("Client");
            NetworkManager.Singleton.StartClient();
            Hide();
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            _hostButton.onClick.AddListener(OnHostButtonClicked);
            _clientButton.onClick.AddListener(OnClientButtonClicked);
        }

        private void RemoveEvents()
        {
            _hostButton.onClick.RemoveListener(OnHostButtonClicked);
            _clientButton.onClick.RemoveListener(OnClientButtonClicked);
        }

        #endregion
    }
}