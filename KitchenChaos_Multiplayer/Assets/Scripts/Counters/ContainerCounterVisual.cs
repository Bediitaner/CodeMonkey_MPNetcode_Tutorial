using System;
using UnityEngine;

namespace Counters
{
    public class ContainerCounterVisual : MonoBehaviour
    {
        #region Contents

        [SerializeField] private ContainerCounter containerCounter;

        #endregion
        
        #region Fields

        private Animator animator;

        private const string OPEN_CLOSE = "OpenClose";

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


        #region Event: OnPlayerGrabbedObject

        private void PlayerGrabbedObjectEvent(object sender, EventArgs e)
        {
            animator.SetTrigger(OPEN_CLOSE);
        }

        #endregion


        #region Events: Add | Remove

        private void AddEvents()
        {
            containerCounter.OnPlayerGrabbedObjectEvent += PlayerGrabbedObjectEvent;
        }

        private void RemoveEvents()
        {
            containerCounter.OnPlayerGrabbedObjectEvent -= PlayerGrabbedObjectEvent;
        }

        #endregion
    }
}