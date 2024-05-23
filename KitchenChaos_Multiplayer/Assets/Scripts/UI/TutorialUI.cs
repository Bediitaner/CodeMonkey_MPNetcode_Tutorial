using System;
using KitchenChaos_Multiplayer.Game;
using KitchenChaos_Multiplayer.Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TutorialUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private TextMeshProUGUI keyMoveUpText;
        [SerializeField] private TextMeshProUGUI keyMoveDownText;
        [SerializeField] private TextMeshProUGUI keyMoveLeftText;
        [SerializeField] private TextMeshProUGUI keyMoveRightText;
        [SerializeField] private TextMeshProUGUI keyInteractText;
        [SerializeField] private TextMeshProUGUI keyInteractAlternateText;
        [SerializeField] private TextMeshProUGUI keyPauseText;
        [SerializeField] private TextMeshProUGUI keyGamepadInteractText;
        [SerializeField] private TextMeshProUGUI keyGamepadInteractAlternateText;
        [SerializeField] private TextMeshProUGUI keyGamepadPauseText;

        #endregion
        
        #region Unity: Start

        private void Start()
        {
            AddEvents();

            UpdateUI();

            Show();
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
        
        
        #region Update: UI

        private void UpdateUI()
        {
            keyMoveUpText.text = GameInput.Instance.GetBindingText(Binding.Move_Up);
            keyMoveDownText.text = GameInput.Instance.GetBindingText(Binding.Move_Down);
            keyMoveLeftText.text = GameInput.Instance.GetBindingText(Binding.Move_Left);
            keyMoveRightText.text = GameInput.Instance.GetBindingText(Binding.Move_Right);
            keyInteractText.text = GameInput.Instance.GetBindingText(Binding.Interact);
            keyInteractAlternateText.text = GameInput.Instance.GetBindingText(Binding.InteractAlternate);
            keyPauseText.text = GameInput.Instance.GetBindingText(Binding.Pause);
            keyGamepadInteractText.text = GameInput.Instance.GetBindingText(Binding.Gamepad_Interact);
            keyGamepadInteractAlternateText.text = GameInput.Instance.GetBindingText(Binding.Gamepad_InteractAlternate);
            keyGamepadPauseText.text = GameInput.Instance.GetBindingText(Binding.Gamepad_Pause);
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

        #region Event: OnLocalPlayerReadyChanged

        private void OnLocalPlayerReadyChanged(object sender, EventArgs e)
        {
            if (KitchenGameManager.Instance.IsLocalPlayerReady())
            {
                Hide();
            }
        }
        
        #endregion

        #region Event: OnKeyBindingChanged

        private void OnBindingRebind(object sender, EventArgs e)
        {
            UpdateUI();
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            GameInput.Instance.OnBindingRebindEvent += OnBindingRebind;
            KitchenGameManager.Instance.OnStateChangedEvent += OnStateChanged;
            KitchenGameManager.Instance.OnLocalPlayerReadyChangedEvent += OnLocalPlayerReadyChanged;
        }

        private void RemoveEvents()
        {
            GameInput.Instance.OnBindingRebindEvent -= OnBindingRebind;
            KitchenGameManager.Instance.OnStateChangedEvent -= OnStateChanged;
            KitchenGameManager.Instance.OnLocalPlayerReadyChangedEvent -= OnLocalPlayerReadyChanged;
        }

        #endregion
    }
}