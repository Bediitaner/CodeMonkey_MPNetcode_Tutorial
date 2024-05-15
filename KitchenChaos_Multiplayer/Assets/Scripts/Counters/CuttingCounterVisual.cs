using System;
using UnityEngine;

namespace Counters
{
    public class CuttingCounterVisual : MonoBehaviour
    {
        #region Contents

        [SerializeField] private CuttingCounter cuttingCounter;

        #endregion

        #region Fields

        private Animator animator;

        private const string CUT = "Cut";

        #endregion

        #region Unity: Awake | Start

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            AddEvents();
        }

        #endregion

        
        #region Event: OnCut

        private void CutEvent(object sender, EventArgs e)
        {
            animator.SetTrigger(CUT);
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            cuttingCounter.OnCutEvent += CutEvent;
        }
        
        private void RemoveEvents()
        {
            cuttingCounter.OnCutEvent -= CutEvent;
        }

        #endregion
    }
}