using System;
using KitchenChaos_Multiplayer.Game;
using KitchenChaos_Multiplayer.Managers;
using Unity.Netcode;
using UnityEngine;

namespace Counters
{
    public class PlatesCounter : BaseCounter
    {
        #region Events

        public event EventHandler OnPlateSpawnedEvent;
        public event EventHandler OnPlateRemovedEvent;

        #endregion

        #region Contents

        [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

        #endregion

        #region Fields

        private float spawnPlateTimer;
        private float spawnPlateTimerMax = 4f;
        private int platesSpawnedAmount;
        private int platesSpawnedAmountMax = 4;

        #endregion


        #region Unity: Update

        private void Update()
        {
            SpawnPlate();
        }

        #endregion

        
        #region Spawn: Plate

        private void SpawnPlate()
        {
            if (!IsServer)
            {
                return;
            }

            spawnPlateTimer += Time.deltaTime;
            if (spawnPlateTimer > spawnPlateTimerMax)
            {
                spawnPlateTimer = 0f;

                if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax)
                {
                    SpawnPlateServerRpc();
                }
            }
        }

        #endregion

        #region ServerRpc: SpawnPlate

        [ServerRpc]
        private void SpawnPlateServerRpc()
        {
            SpawnPlateClientRpc();
        }

        #endregion

        #region ClientRpc: SpawnPlate

        [ClientRpc]
        private void SpawnPlateClientRpc()
        {
            platesSpawnedAmount++;

            OnPlateSpawnedEvent?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        
        #region Override: Interact

        public override void Interact(Player player)
        {
            if (!player.HasKitchenObject())
            {
                // Player is empty handed
                if (platesSpawnedAmount > 0)
                {
                    KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                    InteractLogicServerRpc();
                }
            }
        }

        #endregion

        #region ServerRpc: InteractLogic

        [ServerRpc(RequireOwnership = false)]
        private void InteractLogicServerRpc()
        {
            InteractLogicClientRpc();
        }

        #endregion

        #region ClientRpc: InteractLogic

        [ClientRpc]
        private void InteractLogicClientRpc()
        {
            platesSpawnedAmount--;

            OnPlateRemovedEvent?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}