using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GameStartCountdownUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private TextMeshProUGUI countdownText;

        #endregion
        
        #region Fields

        private Animator animator;
        private int previousCountdownNumber;

        private const string NUMBER_POPUP = "NumberPopup";

        #endregion

        #region Unity: Awake | Start | Update

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            AddEvents();
            Hide();
        }
        
        private void Update()
        {
            int countdownNumber = Mathf.CeilToInt(KitchenGameManager.Instance.GetCountdownToStartTimer());
            countdownText.text = countdownNumber.ToString();

            if (previousCountdownNumber != countdownNumber)
            {
                previousCountdownNumber = countdownNumber;
                animator.SetTrigger(NUMBER_POPUP);
                SoundManager.Instance.PlayCountdownSound();
            }
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
        
        
        #region Event: OnStateChanged

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (KitchenGameManager.Instance.IsCountdownToStartActive())
            {
                Show();
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