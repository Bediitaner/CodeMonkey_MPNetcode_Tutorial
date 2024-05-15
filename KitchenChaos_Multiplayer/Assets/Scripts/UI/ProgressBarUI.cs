using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProgressBarUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private GameObject hasProgressGameObject;
        [SerializeField] private Image barImage;

        #endregion

        #region Fields

        private IHasProgress hasProgress;

        #endregion
        
        #region Unity: Start

        private void Start()
        {
            hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
            if (hasProgress == null)
            {
                Debug.LogError("Game Object " + hasProgressGameObject + " does not have a component that implements IHasProgress!");
            }

            AddEvents();

            barImage.fillAmount = 0f;

            Hide();
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

        
        #region Event: OnProgressChanged

        private void OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
        {
            barImage.fillAmount = e.progressNormalized;

            if (e.progressNormalized == 0f || e.progressNormalized == 1f)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        #endregion
        
        #region Events: Add | Remove

        private void AddEvents()
        {
            hasProgress.OnProgressChangedEvent += OnProgressChanged; 
        }

        private void RemoveEvents()
        {
            hasProgress.OnProgressChangedEvent -= OnProgressChanged;
        }

        #endregion
    }
}