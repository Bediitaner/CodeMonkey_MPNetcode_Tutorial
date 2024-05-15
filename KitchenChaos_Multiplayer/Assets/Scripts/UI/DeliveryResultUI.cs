using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DeliveryResultUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Color successColor;
        [SerializeField] private Color failedColor;
        [SerializeField] private Sprite successSprite;
        [SerializeField] private Sprite failedSprite;

        #endregion

        #region Fields

        private Animator animator;

        private const string POPUP = "Popup";

        #endregion

        #region Unity: Awake | Start

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            AddEvents();
            
            gameObject.SetActive(false);
        }

        #endregion

        
        #region Event: OnRecipeFailed

        private void OnRecipeFailed(object sender, EventArgs e)
        {
            gameObject.SetActive(true);
            animator.SetTrigger(POPUP);
            backgroundImage.color = failedColor;
            iconImage.sprite = failedSprite;
            messageText.text = "DELIVERY\nFAILED";
        }
        #endregion

        #region Event: OnRecipeSuccess

        private void OnRecipeSuccess(object sender, EventArgs e)
        {
            gameObject.SetActive(true);
            animator.SetTrigger(POPUP);
            backgroundImage.color = successColor;
            iconImage.sprite = successSprite;
            messageText.text = "DELIVERY\nSUCCESS";
        }
        

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            DeliveryManager.Instance.OnRecipeSuccessEvent += OnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFailedEvent += OnRecipeFailed;
        }
        
        private void RemoveEvents()
        {
            DeliveryManager.Instance.OnRecipeSuccessEvent -= OnRecipeSuccess;
            DeliveryManager.Instance.OnRecipeFailedEvent -= OnRecipeFailed;
        }

        #endregion
    }
}