using System;
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
            keyMoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
            keyMoveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
            keyMoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
            keyMoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
            keyInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
            keyInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
            keyPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
            keyGamepadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
            keyGamepadInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
            keyGamepadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
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

        #region Event: OnKeyBindingChanged

        private void OnBindingRebind(object sender, EventArgs e)
        {
            UpdateUI();
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            GameInput.Instance.OnBindingRebind += OnBindingRebind;
            KitchenGameManager.Instance.OnStateChanged += OnStateChanged;
        }

        private void RemoveEvents()
        {
            GameInput.Instance.OnBindingRebind -= OnBindingRebind;
            KitchenGameManager.Instance.OnStateChanged -= OnStateChanged;
        }

        #endregion
    }
}