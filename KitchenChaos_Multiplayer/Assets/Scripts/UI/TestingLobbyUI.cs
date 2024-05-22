using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TestingLobbyUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Button _btnCreateGame;
        [SerializeField] private Button _btnJoinGame;

        #endregion

        #region Unity: Awake

        private void Awake()
        {
            AddEvents();
        }

        #endregion


        #region Event: OnCreateGameClicked

        private void OnCreateGameClicked()
        {
            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Scene.CharacterSelectScene);
        }

        #endregion
    
        #region Event: OnCreateGameClicked

        private void OnJoinGameClicked()
        {
            KitchenGameMultiplayer.Instance.StartClient();
        }

        #endregion


        #region Events: Add | Remove

        private void AddEvents()
        {
            _btnCreateGame.onClick.AddListener(OnCreateGameClicked);
            _btnJoinGame.onClick.AddListener(OnJoinGameClicked);
        }


        private void RemoveEvents()
        {
            _btnCreateGame.onClick.RemoveListener(OnCreateGameClicked);
            _btnJoinGame.onClick.RemoveListener(OnJoinGameClicked);
        }

        #endregion
    }
}