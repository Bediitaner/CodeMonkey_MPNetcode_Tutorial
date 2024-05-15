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
            Player.LocalInstance.OnSelectedCounterChangedEvent += SelectedCounterChangedEvent;
        }
        else
        {
            Player.OnAnyPlayerSpawnedEvent += AnyPlayerSpawnedEvent;
        }
    }

    #endregion


    #region Event: OnAnyPlayerSpawned

    private void AnyPlayerSpawnedEvent(object sender, EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChangedEvent -= SelectedCounterChangedEvent;
            Player.LocalInstance.OnSelectedCounterChangedEvent += SelectedCounterChangedEvent;
        }
    }

    #endregion

    #region Event: OnSelectedCounterChanged

    private void SelectedCounterChangedEvent(object sender, OnSelectedCounterChangedEventArgs e)
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