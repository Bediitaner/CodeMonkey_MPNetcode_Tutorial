using KitchenChaos_Multiplayer.Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        #endregion
        
        #region Unity: Awake

        private void Awake()
        {
            playButton.onClick.AddListener(() => { Loader.Load(Scene.LobbyScene); });
            quitButton.onClick.AddListener(() => { Application.Quit(); });

            Time.timeScale = 1f;
        }

        #endregion
    }
}