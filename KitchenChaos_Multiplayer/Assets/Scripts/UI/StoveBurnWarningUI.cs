using Counters;
using UnityEngine;

namespace UI
{
    public class StoveBurnWarningUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private StoveCounter stoveCounter;

        #endregion

        #region Unity: Start

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
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        #endregion

        
        #region Event: OnProgressChanged

        private void OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
        {
            float burnShowProgressAmount = .5f;
            bool show = stoveCounter.IsFried() && e.progressNormalized >= burnShowProgressAmount;

            if (show)
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
            stoveCounter.OnProgressChanged += OnProgressChanged;
        }

        private void RemoveEvents()
        {
            stoveCounter.OnProgressChanged -= OnProgressChanged;
        }

        #endregion
    }
}