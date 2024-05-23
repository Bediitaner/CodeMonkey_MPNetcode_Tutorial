using System;
using KitchenChaos_Multiplayer.Game;
using KitchenChaos_Multiplayer.Managers;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameOverUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private TextMeshProUGUI recipesDeliveredText;
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
            playAgainButton.Select();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        #endregion
        
        
        #region Event: OnStateChanged

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (KitchenGameManager.Instance.IsGameOver())
            {
                Show();

                recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
            }
            else
            {
                Hide();
            }
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            KitchenGameManager.Instance.OnStateChangedEvent += OnStateChanged;
        }

        private void RemoveEvents()
        {
            KitchenGameManager.Instance.OnStateChangedEvent -= OnStateChanged;
        }

        #endregion
    }
}
