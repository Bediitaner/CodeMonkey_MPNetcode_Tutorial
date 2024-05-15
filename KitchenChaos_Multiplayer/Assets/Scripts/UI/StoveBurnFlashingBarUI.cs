using Counters;
using UnityEngine;

namespace UI
{
    public class StoveBurnFlashingBarUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private StoveCounter stoveCounter;

        #endregion

        #region Fields

        private Animator animator;

        private const string IS_FLASHING = "IsFlashing";

        #endregion


        #region Unity: Awake | Start

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            AddEvents();

            animator.SetBool(IS_FLASHING, false);
        }

        #endregion

        
        #region Event: OnProgressChanged

        private void OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
        {
            float burnShowProgressAmount = .5f;
            bool show = stoveCounter.IsFried() && e.progressNormalized >= burnShowProgressAmount;

            animator.SetBool(IS_FLASHING, show);
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            stoveCounter.OnProgressChangedEvent += OnProgressChanged;
        }

        private void RemoveEvents()
        {
            stoveCounter.OnProgressChangedEvent -= OnProgressChanged;
        }

        #endregion
    }
}