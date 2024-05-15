using System;
using System.Collections.Generic;
using UnityEngine;

namespace Counters
{
    public class PlatesCounterVisual : MonoBehaviour
    {
        #region Contents

        [SerializeField] private PlatesCounter platesCounter;
        [SerializeField] private Transform counterTopPoint;
        [SerializeField] private Transform plateVisualPrefab;

        #endregion

        #region Fields

        private List<GameObject> plateVisualGameObjectList;

        #endregion

        #region Unity: Awake | Start

        private void Awake()
        {
            plateVisualGameObjectList = new List<GameObject>();
        }

        private void Start()
        {
            AddEvents();
        }

        #endregion


        #region Event: OnPlateSpawned

        private void OnPlateSpawned(object sender, EventArgs e)
        {
            Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);

            float plateOffsetY = .1f;
            plateVisualTransform.localPosition = new Vector3(0, plateOffsetY * plateVisualGameObjectList.Count, 0);

            plateVisualGameObjectList.Add(plateVisualTransform.gameObject);
        }

        #endregion

        #region Event: OnPlateRemoved

        private void OnPlateRemoved(object sender, EventArgs e)
        {
            GameObject plateGameObject = plateVisualGameObjectList[plateVisualGameObjectList.Count - 1];
            plateVisualGameObjectList.Remove(plateGameObject);
            Destroy(plateGameObject);
        }

        #endregion

        #region Events: Add | Remove

        private void AddEvents()
        {
            platesCounter.OnPlateSpawnedEvent += OnPlateSpawned;
            platesCounter.OnPlateRemovedEvent += OnPlateRemoved;
        }

        private void RemoveEvents()
        {
            platesCounter.OnPlateSpawnedEvent -= OnPlateSpawned;
            platesCounter.OnPlateRemovedEvent -= OnPlateRemoved;
        }

        #endregion
    }
}