using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OptionsUI : MonoBehaviour
    {
        #region Singleton

        public static OptionsUI Instance { get; private set; }

        #endregion


        #region Contents

        [SerializeField] private Button soundEffectsButton;
        [SerializeField] private Button musicButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button moveUpButton;
        [SerializeField] private Button moveDownButton;
        [SerializeField] private Button moveLeftButton;
        [SerializeField] private Button moveRightButton;
        [SerializeField] private Button interactButton;
        [SerializeField] private Button interactAlternateButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button gamepadInteractButton;
        [SerializeField] private Button gamepadInteractAlternateButton;
        [SerializeField] private Button gamepadPauseButton;
        [SerializeField] private TextMeshProUGUI soundEffectsText;
        [SerializeField] private TextMeshProUGUI musicText;
        [SerializeField] private TextMeshProUGUI moveUpText;
        [SerializeField] private TextMeshProUGUI moveDownText;
        [SerializeField] private TextMeshProUGUI moveLeftText;
        [SerializeField] private TextMeshProUGUI moveRightText;
        [SerializeField] private TextMeshProUGUI interactText;
        [SerializeField] private TextMeshProUGUI interactAlternateText;
        [SerializeField] private TextMeshProUGUI pauseText;
        [SerializeField] private TextMeshProUGUI gamepadInteractText;
        [SerializeField] private TextMeshProUGUI gamepadInteractAlternateText;
        [SerializeField] private TextMeshProUGUI gamepadPauseText;
        [SerializeField] private Transform pressToRebindKeyTransform;

        #endregion

        #region Fields

        private Action onCloseButtonAction;

        #endregion

        #region Unity: Awake | Start

        private void Awake()
        {
            Instance = this;

            AddEvents();
        }

        private void Start()
        {
            UpdateUI();

            HidePressToRebindKey();
            Hide();
        }

        #endregion


        #region Show | Hide

        public void Show(Action onCloseButtonAction)
        {
            this.onCloseButtonAction = onCloseButtonAction;

            gameObject.SetActive(true);

            soundEffectsButton.Select();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        #endregion


        #region Update: UI

        private void UpdateUI()
        {
            soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
            musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

            moveUpText.text = GameInput.Instance.GetBindingText(Binding.Move_Up);
            moveDownText.text = GameInput.Instance.GetBindingText(Binding.Move_Down);
            moveLeftText.text = GameInput.Instance.GetBindingText(Binding.Move_Left);
            moveRightText.text = GameInput.Instance.GetBindingText(Binding.Move_Right);
            interactText.text = GameInput.Instance.GetBindingText(Binding.Interact);
            interactAlternateText.text = GameInput.Instance.GetBindingText(Binding.InteractAlternate);
            pauseText.text = GameInput.Instance.GetBindingText(Binding.Pause);
            gamepadInteractText.text = GameInput.Instance.GetBindingText(Binding.Gamepad_Interact);
            gamepadInteractAlternateText.text = GameInput.Instance.GetBindingText(Binding.Gamepad_InteractAlternate);
            gamepadPauseText.text = GameInput.Instance.GetBindingText(Binding.Gamepad_Pause);
        }

        #endregion


        #region Show | Hide: PressToRebindKey

        private void ShowPressToRebindKey()
        {
            pressToRebindKeyTransform.gameObject.SetActive(true);
        }

        private void HidePressToRebindKey()
        {
            pressToRebindKeyTransform.gameObject.SetActive(false);
        }

        #endregion

        #region RebindBinding

        private void RebindBinding(Binding binding)
        {
            ShowPressToRebindKey();
            GameInput.Instance.RebindBinding(binding, () =>
            {
                HidePressToRebindKey();
                UpdateUI();
            });
        }

        #endregion


        #region Event: OnGameUnpaused

        private void OnGameUnpaused(object sender, EventArgs e)
        {
            Hide();
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            soundEffectsButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.ChangeVolume();
                UpdateUI();
            });
            musicButton.onClick.AddListener(() =>
            {
                MusicManager.Instance.ChangeVolume();
                UpdateUI();
            });
            closeButton.onClick.AddListener(() =>
            {
                Hide();
                onCloseButtonAction();
            });

            moveUpButton.onClick.AddListener(() => { RebindBinding(Binding.Move_Up); });
            moveDownButton.onClick.AddListener(() => { RebindBinding(Binding.Move_Down); });
            moveLeftButton.onClick.AddListener(() => { RebindBinding(Binding.Move_Left); });
            moveRightButton.onClick.AddListener(() => { RebindBinding(Binding.Move_Right); });
            interactButton.onClick.AddListener(() => { RebindBinding(Binding.Interact); });
            interactAlternateButton.onClick.AddListener(() => { RebindBinding(Binding.InteractAlternate); });
            pauseButton.onClick.AddListener(() => { RebindBinding(Binding.Pause); });
            gamepadInteractButton.onClick.AddListener(() => { RebindBinding(Binding.Gamepad_Interact); });
            gamepadInteractAlternateButton.onClick.AddListener(() => { RebindBinding(Binding.Gamepad_InteractAlternate); });
            gamepadPauseButton.onClick.AddListener(() => { RebindBinding(Binding.Gamepad_Pause); });

            KitchenGameManager.Instance.OnGameUnpaused += OnGameUnpaused;
        }

        private void RemoveEvents()
        {
            KitchenGameManager.Instance.OnGameUnpaused -= OnGameUnpaused;
        }

        #endregion
    }
}