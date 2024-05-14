using UnityEngine;

namespace Counters
{
    public class StoveCounterVisual : MonoBehaviour
    {
        #region Contents

        [SerializeField] private StoveCounter stoveCounter;
        [SerializeField] private GameObject stoveOnGameObject;
        [SerializeField] private GameObject particlesGameObject;

        #endregion

        
        #region Unity: Start

        private void Start()
        {
            AddEvents();
        }

        #endregion

        
        #region Event: OnStateChanged

        private void OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
        {
            bool showVisual = e.state == State.Frying || e.state == State.Fried;
                stoveOnGameObject.SetActive(showVisual);
            particlesGameObject.SetActive(showVisual);
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            stoveCounter.OnStateChanged += OnStateChanged;
        }

        private void RemoveEvents()
        {
            stoveCounter.OnStateChanged -= OnStateChanged;
        }

        #endregion
    }
}