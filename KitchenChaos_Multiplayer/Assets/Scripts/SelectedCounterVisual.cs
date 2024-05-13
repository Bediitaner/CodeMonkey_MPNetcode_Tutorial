using System;
using Counters;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    #region Contents

    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;

    #endregion


    #region Unity: Start

    private void Start()
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChanged += OnSelectedCounterChanged;
        }
        else
        {
            Player.OnAnyPlayerSpawned += OnAnyPlayerSpawned;
        }
    }

    #endregion


    #region Event: OnAnyPlayerSpawned

    private void OnAnyPlayerSpawned(object sender, EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChanged -= OnSelectedCounterChanged;
            Player.LocalInstance.OnSelectedCounterChanged += OnSelectedCounterChanged;
        }
    }

    #endregion

    #region Event: OnSelectedCounterChanged

    private void OnSelectedCounterChanged(object sender, OnSelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == baseCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    #endregion

    #region Show | Hide

    private void Show()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }

    #endregion
}